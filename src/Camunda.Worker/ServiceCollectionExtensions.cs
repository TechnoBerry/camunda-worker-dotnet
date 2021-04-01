using System;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Camunda.Worker
{
    public static class CamundaWorkerServiceCollectionExtensions
    {
        public static ICamundaWorkerBuilder AddCamundaWorker(
            this IServiceCollection services,
            Action<CamundaWorkerOptions> configureDelegate
        )
        {
            services.AddOptions<CamundaWorkerOptions>()
                .Configure(configureDelegate);
            services.AddExternalTaskClient();

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
