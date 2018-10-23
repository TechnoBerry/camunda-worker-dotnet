// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddCamundaWorker(this IServiceCollection services)
        {
            return services;
        }
    }
}
