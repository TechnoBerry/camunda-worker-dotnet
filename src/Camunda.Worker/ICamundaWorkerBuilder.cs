#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface ICamundaWorkerBuilder
    {
        IServiceCollection Services { get; }

        ICamundaWorkerBuilder AddFactoryProvider<TProvider>() where TProvider : class, IHandlerFactoryProvider;

        ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider;

        ICamundaWorkerBuilder AddTaskSelector<TSelector>() where TSelector : class, IExternalTaskSelector;

        ICamundaWorkerBuilder AddExceptionHandler<THandler>() where THandler : class, IExceptionHandler;

        ICamundaWorkerBuilder AddHandlerDescriptor(HandlerDescriptor descriptor);
    }
}
