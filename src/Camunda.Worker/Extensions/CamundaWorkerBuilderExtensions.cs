// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
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

            var topicAttributes = typeof(T).GetCustomAttributes<HandlerTopicAttribute>();

            return topicAttributes.Aggregate(builder, (acc, topicAttribute) => acc.Add(
                new HandlerDescriptor(topicAttribute.TopicName, HandlerFactory<T>)
            ));
        }

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }
    }
}
