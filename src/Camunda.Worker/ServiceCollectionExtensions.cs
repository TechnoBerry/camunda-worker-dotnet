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
            string workerId,
            int numberOfWorkers = Constants.MinimumParallelExecutors
        )
        {
            Guard.NotEmptyAndNotNull(workerId, nameof(workerId));
            Guard.GreaterThanOrEqual(numberOfWorkers, Constants.MinimumParallelExecutors, nameof(numberOfWorkers));

            services.AddSingleton(new CamundaWorkerOptions(workerId));
            services.AddOptions<SelectorOptions>();
            services.AddExternalTaskClient();

            services.TryAddTransient<ITopicsProvider, StaticTopicsProvider>();
            services.TryAddTransient<IExternalTaskSelector, ExternalTaskSelector>();
            services.TryAddTransient<IContextFactory, ContextFactory>();
            services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();
            services.TryAddTransient<IExternalTaskRouter, ExternalTaskRouter>();
            services.TryAddSingleton<IEndpointProvider, TopicBasedEndpointProvider>();
            services.TryAddSingleton(new WorkerHandlerDescriptor(PipelineBuilder.RouteAsync));
            services.AddHostedService(provider => new WorkerHostedService(provider, numberOfWorkers));

            return new CamundaWorkerBuilder(services, workerId);
        }
    }
}
