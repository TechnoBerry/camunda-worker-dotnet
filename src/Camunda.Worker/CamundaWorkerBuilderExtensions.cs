using System;
using System.Reflection;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public static class CamundaWorkerBuilderExtensions
    {
        public static ICamundaWorkerBuilder AddHandler<T>(
            this ICamundaWorkerBuilder builder,
            Action<HandlerMetadata>? configureMetadata = null
        )
            where T : class, IExternalTaskHandler
        {
            Guard.NotNull(builder, nameof(builder));

            var metadata = CollectMetadataFromAttributes(typeof(T));
            configureMetadata?.Invoke(metadata);

            return builder.AddHandler<T>(metadata);
        }

        private static HandlerMetadata CollectMetadataFromAttributes(Type handlerType)
        {
            var topicsAttribute = handlerType.GetCustomAttribute<HandlerTopicsAttribute>();

            if (topicsAttribute == null)
            {
                throw new Exception($"\"{handlerType.FullName}\" doesn't provide any \"HandlerTopicsAttribute\"");
            }

            var variablesAttribute = handlerType.GetCustomAttribute<HandlerVariablesAttribute>();

            return new HandlerMetadata(topicsAttribute.TopicNames, topicsAttribute.LockDuration)
            {
                LocalVariables = variablesAttribute?.LocalVariables ?? false,
                Variables = variablesAttribute?.Variables
            };
        }

        public static ICamundaWorkerBuilder AddHandler<T>(this ICamundaWorkerBuilder builder, HandlerMetadata metadata)
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
}
