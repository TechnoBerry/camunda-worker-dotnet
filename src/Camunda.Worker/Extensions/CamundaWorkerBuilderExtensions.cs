// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
            var services = builder.Services;
            services.AddScoped<T>();

            var handlerDescriptors = MakeDescriptors<T>();

            return handlerDescriptors.Aggregate(builder, (acc, descriptor) => acc.Add(descriptor));
        }

        private static IEnumerable<HandlerDescriptor> MakeDescriptors<T>()
            where T : class, IExternalTaskHandler
        {
            return typeof(T).GetCustomAttributes<HandlerTopicAttribute>()
                .Select(attribute => new HandlerDescriptor(attribute.TopicName, HandlerFactory<T>));
        }

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }
    }
}
