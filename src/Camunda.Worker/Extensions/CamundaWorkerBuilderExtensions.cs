#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
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

            var services = builder.Services;
            services.AddScoped<T>();

            var handlerMetadata = CollectMetadataFromAttributes(typeof(T));
            var handlerDescriptors = MakeDescriptors(HandlerFactory<T>, handlerMetadata);

            return handlerDescriptors.Aggregate(builder, (acc, descriptor) => acc.AddHandlerDescriptor(descriptor));
        }

        private static IEnumerable<HandlerDescriptor> MakeDescriptors(HandlerFactory factory, HandlerMetadata metadata)
        {
            return metadata.TopicNames.Select(topicName =>
            {
                var descriptor = new HandlerDescriptor(topicName, factory)
                {
                    LockDuration = metadata.LockDuration,
                    LocalVariables = metadata.LocalVariables,
                    Variables = metadata.Variables
                };

                return descriptor;
            });
        }

        private static HandlerMetadata CollectMetadataFromAttributes(Type handlerType)
        {
            var topicsAttribute = handlerType.GetCustomAttribute<HandlerTopicsAttribute>();

            if (topicsAttribute == null)
            {
                throw new Exception($"\"{handlerType.FullName}\" doesn't provide any \"HandlerTopicsAttribute\"");
            }

            var variablesAttribute = handlerType.GetCustomAttribute<HandlerVariablesAttribute>();

            return new HandlerMetadata
            {
                TopicNames = topicsAttribute.TopicNames,
                LockDuration = topicsAttribute.LockDuration,
                LocalVariables = variablesAttribute?.LocalVariables ?? false,
                Variables = variablesAttribute?.Variables
            };
        }

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }
    }
}
