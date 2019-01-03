// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var services = builder.Services;
            services.AddScoped<T>();

            var handlerDescriptors = MakeDescriptors<T>();

            return handlerDescriptors.Aggregate(builder, (acc, descriptor) => acc.Add(descriptor));
        }

        private static IEnumerable<HandlerDescriptor> MakeDescriptors<T>()
            where T : class, IExternalTaskHandler
        {
            var handlerType = typeof(T);
            
            return handlerType.GetCustomAttributes<HandlerTopicAttribute>()
                .Select(attribute =>
                {
                    var descriptor = new HandlerDescriptor(attribute.TopicName, HandlerFactory<T>);

                    var variablesAttribute = handlerType.GetCustomAttribute<HandlerVariablesAttribute>();

                    descriptor.LocalVariables = variablesAttribute?.LocalVariables ?? false;
                    descriptor.Variables = variablesAttribute?.Variables?.ToList();
                    
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
