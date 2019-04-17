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
            var handlerDescriptor = new HandlerDescriptor(HandlerFactory<T>, handlerMetadata);

            return builder.AddHandlerDescriptor(handlerDescriptor);
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

        private static IExternalTaskHandler HandlerFactory<T>(IServiceProvider provider)
            where T : class, IExternalTaskHandler
        {
            return provider.GetRequiredService<T>();
        }
    }
}
