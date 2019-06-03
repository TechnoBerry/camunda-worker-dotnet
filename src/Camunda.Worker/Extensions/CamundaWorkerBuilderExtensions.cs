using System;
using System.Reflection;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Extensions
{
    public static class CamundaWorkerBuilderExtensions
    {
        public static ICamundaWorkerBuilder AddHandler<T>(this ICamundaWorkerBuilder builder)
            where T : class, IExternalTaskHandler
        {
            Guard.NotNull(builder, nameof(builder));

            var metadata = CollectMetadataFromAttributes(typeof(T));

            return builder.AddHandler<T>(metadata);
        }

        private static HandlerMetadata CollectMetadataFromAttributes(Type handlerType)
        {
            var topicsAttribute = handlerType.GetCustomAttribute<TopicsAttribute>();

            if (topicsAttribute == null)
            {
                throw new Exception($"\"{handlerType.FullName}\" doesn't provide any \"TopicsAttribute\"");
            }

            var variablesAttribute = handlerType.GetCustomAttribute<VariablesAttribute>();

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
            services.AddScoped<T>();

            return builder.AddHandler(HandlerFactory<T>, metadata);
        }

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }

        public static ICamundaWorkerBuilder AddHandler(this ICamundaWorkerBuilder builder,
            HandlerFactory factory,
            HandlerMetadata metadata)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(factory, nameof(factory));
            Guard.NotNull(metadata, nameof(metadata));

            var handlerDescriptor = new HandlerDescriptor(factory, metadata);

            return builder.AddHandlerDescriptor(handlerDescriptor);
        }
    }
}
