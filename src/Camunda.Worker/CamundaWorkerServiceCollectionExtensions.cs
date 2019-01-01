// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Api;
using Camunda.Worker.Core;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(this IServiceCollection services,
            Action<CamundaWorkerOptions> configureDelegate)
        {
            services.AddOptions<CamundaWorkerOptions>()
                .Configure(configureDelegate);
            services.AddHttpClient<ICamundaApiClient, CamundaApiClient>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<CamundaWorkerOptions>>().Value;
                    client.BaseAddress = options.BaseUri;
                });

            services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();

            return services.AddCamundaWorkerCore();
        }

        public static ICamundaWorkerBuilder AddCamundaWorkerCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IHandlerFactoryProvider, DefaultHandlerFactoryProvider>();
            services.TryAddTransient<IExternalTaskHandler, CompositeExternalTaskHandler>();

            return new CamundaWorkerBuilder(services);
        }
    }
}
