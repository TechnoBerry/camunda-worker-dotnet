#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilder : ICamundaWorkerBuilder
    {
        public CamundaWorkerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public ICamundaWorkerBuilder AddFactoryProvider<TProvider>() where TProvider : class, IHandlerFactoryProvider
        {
            Services.AddTransient<IHandlerFactoryProvider, TProvider>();

            return this;
        }

        public ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider
        {
            Services.AddTransient<ITopicsProvider, TProvider>();

            return this;
        }

        public ICamundaWorkerBuilder AddExceptionHandler<THandler>() where THandler : class, IExceptionHandler
        {
            Services.AddTransient<IExceptionHandler, THandler>();

            return this;
        }

        public ICamundaWorkerBuilder AddHandlerDescriptor(HandlerDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            Services.AddSingleton(descriptor);
            return this;
        }
    }
}
