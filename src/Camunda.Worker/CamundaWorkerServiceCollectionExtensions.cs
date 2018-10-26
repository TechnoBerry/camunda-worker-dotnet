// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Camunda.Worker.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(this IServiceCollection services)
        {
            services.TryAddSingleton<IHandlerFactoryProvider, DefaultHandlerFactoryProvider>();
            services.TryAddTransient<IExternalTaskExecutor, DefaultExternalTaskExecutor>();

            return new CamundaWorkerBuilder(services);
        }
    }
}
