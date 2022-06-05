using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker;

public static class CamundaWorkerServiceCollectionExtensions
{
    public static ICamundaWorkerBuilder AddCamundaWorker(
        this IServiceCollection services,
        WorkerIdString workerId,
        int numberOfWorkers = Constants.MinimumParallelExecutors
    )
    {
        Guard.GreaterThanOrEqual(numberOfWorkers, Constants.MinimumParallelExecutors, nameof(numberOfWorkers));

        services.AddOptions<FetchAndLockOptions>(workerId.Value);
        services.AddOptions<WorkerEvents>();
        services.TryAddTransient<ITopicsProvider, StaticTopicsProvider>();
        services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();
        services.TryAddSingleton<IEndpointProvider, TopicBasedEndpointProvider>();
        services.AddHostedService(provider => new WorkerHostedService(provider, numberOfWorkers));

        return new CamundaWorkerBuilder(services, workerId)
            .AddFetchAndLockRequestProvider((workerId, provider) => new LegacyFetchAndLockRequestProvider(
                workerId,
                provider.GetRequiredService<ITopicsProvider>(),
                provider.GetRequiredService<IOptionsMonitor<FetchAndLockOptions>>()
            ))
            .ConfigurePipeline(_ => { });
    }
}
