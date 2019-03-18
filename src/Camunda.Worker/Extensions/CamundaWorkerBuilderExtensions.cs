#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            var handlerDescriptors = MakeDescriptors<T>();

            return handlerDescriptors.Aggregate(builder, (acc, descriptor) => acc.AddHandlerDescriptor(descriptor));
        }

        private static IEnumerable<HandlerDescriptor> MakeDescriptors<T>()
            where T : class, IExternalTaskHandler
        {
            var handlerType = typeof(T);
            var topicAttributes = handlerType.GetCustomAttributes<HandlerTopicAttribute>().ToList();

            if (!topicAttributes.Any())
            {
                throw new Exception($"\"{handlerType.FullName}\" doesn't provide any \"HandlerTopicAttribute\"");
            }

            var variablesAttribute = handlerType.GetCustomAttribute<HandlerVariablesAttribute>();
            var localVariables = variablesAttribute?.LocalVariables ?? false;
            var variables = variablesAttribute?.Variables?.ToList();

            return topicAttributes.Select(attribute =>
            {
                var descriptor = new HandlerDescriptor(attribute.TopicName, HandlerFactory<T>)
                {
                    LockDuration = attribute.LockDuration,
                    LocalVariables = localVariables,
                    Variables = variables
                };

                return descriptor;
            });
        }

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }
    }
}
