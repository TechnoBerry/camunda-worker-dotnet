using System;
using System.Reflection;
using System.Threading.Tasks;
using Camunda.Worker.Endpoints;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker;

public static class CamundaWorkerBuilderExtensions
{
    public static ICamundaWorkerBuilder AddHandler<T>(
        this ICamundaWorkerBuilder builder,
        Action<EndpointMetadata>? configureMetadata = null
    )
        where T : class, IExternalTaskHandler
    {
        Guard.NotNull(builder, nameof(builder));

        var metadata = CollectMetadataFromAttributes(typeof(T));
        configureMetadata?.Invoke(metadata);

        return builder.AddHandler<T>(metadata);
    }

    private static EndpointMetadata CollectMetadataFromAttributes(Type handlerType)
    {
        var topicsAttribute = handlerType.GetCustomAttribute<HandlerTopicsAttribute>();

        if (topicsAttribute == null)
        {
            throw new ArgumentException(
                $"\"{handlerType.FullName}\" doesn't provide any \"HandlerTopicsAttribute\"",
                nameof(handlerType)
            );
        }

        var variablesAttribute = handlerType.GetCustomAttribute<HandlerVariablesAttribute>();

        return new EndpointMetadata(topicsAttribute.TopicNames, topicsAttribute.LockDuration)
        {
            LocalVariables = variablesAttribute?.LocalVariables ?? false,
            Variables =  variablesAttribute?.AllVariables ?? false ? null : variablesAttribute?.Variables,
            IncludeExtensionProperties = topicsAttribute.IncludeExtensionProperties
        };
    }

    public static ICamundaWorkerBuilder AddHandler<T>(this ICamundaWorkerBuilder builder, EndpointMetadata metadata)
        where T : class, IExternalTaskHandler
    {
        Guard.NotNull(builder, nameof(builder));
        Guard.NotNull(metadata, nameof(metadata));

        var services = builder.Services;
        services.AddTransient<T>();

        return builder.AddHandler(HandlerDelegate<T>, metadata);
    }

    private static Task HandlerDelegate<T>(IExternalTaskContext context)
        where T : class, IExternalTaskHandler
    {
        var handler = context.ServiceProvider.GetRequiredService<T>();
        var invoker = new HandlerInvoker(handler, context);
        return invoker.InvokeAsync();
    }
}
