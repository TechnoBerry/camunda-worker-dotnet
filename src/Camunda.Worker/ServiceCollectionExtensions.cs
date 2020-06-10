using System;
using Camunda.Worker.Client;
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
            services.AddExternalTaskClient()
                .ConfigureHttpClient((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<CamundaWorkerOptions>>().Value;
                    client.BaseAddress = options.BaseUri;
                    foreach (var header in options.CustomHttpHeaders.Keys)
                    {
                        client.DefaultRequestHeaders.Add(header, options.CustomHttpHeaders[header]);
                    }

                });

            services.TryAddTransient<ITopicsProvider, StaticTopicsProvider>();
            services.TryAddTransient<IExternalTaskSelector, ExternalTaskSelector>();
            services.TryAddTransient<IContextFactory, ContextFactory>();
            services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();
            services.TryAddTransient<IExternalTaskRouter, ExternalTaskRouter>();
            services.TryAddSingleton<IEndpointProvider, TopicBasedEndpointProvider>();
            services.TryAddSingleton(new PipelineDescriptor(PipelineBuilder.RouteAsync));
            services.AddHostedService<WorkerHostedService>();


            return new CamundaWorkerBuilder(services);
        }
    }
}
