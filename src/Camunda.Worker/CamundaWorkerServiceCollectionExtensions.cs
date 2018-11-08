// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Api;
using Camunda.Worker.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(this IServiceCollection services,
            Action<CamundaWorkerOptions> configureDelegate)
        {
            var options = new CamundaWorkerOptions();
            configureDelegate(options);

            services.AddRefitClient<ICamundaApiClient>(new RefitSettings
                {
                    JsonSerializerSettings = MakeJsonSerializerSettings()
                })
                .ConfigureHttpClient(client => { client.BaseAddress = options.BaseUri; });

            services.TryAddSingleton<IHandlerFactoryProvider, DefaultHandlerFactoryProvider>();
            services.TryAddTransient<IExternalTaskExecutor, DefaultExternalTaskExecutor>();

            return new CamundaWorkerBuilder(services);
        }

        private static JsonSerializerSettings MakeJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}
