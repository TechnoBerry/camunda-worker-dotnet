// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Camunda.Worker.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(this IServiceCollection services)
        {
            services.AddSingleton<IHandlerFactoryProvider, DefaultHandlerFactoryProvider>();
            services.AddTransient<IExternalTaskExecutor, DefaultExternalTaskExecutor>();

            return new CamundaWorkerBuilder(services);
        }
    }
}
