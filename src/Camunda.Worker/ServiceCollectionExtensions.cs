using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker;

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

        services.AddOptions<FetchAndLockOptions>().Configure(options => { options.WorkerId = workerId; });
        services.AddOptions<WorkerEvents>();
        services.TryAddTransient<ITopicsProvider, StaticTopicsProvider>();
        services.TryAddTransient<ICamundaWorker, DefaultCamundaWorker>();
        services.TryAddSingleton<IEndpointProvider, TopicBasedEndpointProvider>();
        services.AddHostedService(provider => new WorkerHostedService(provider, numberOfWorkers));

        return new CamundaWorkerBuilder(services, workerId)
            .AddFetchAndLockRequestProvider((_, provider) => new LegacyFetchAndLockRequestProvider(
                provider.GetRequiredService<ITopicsProvider>(),
                provider.GetRequiredService<IOptions<FetchAndLockOptions>>()
            ))
            .ConfigurePipeline(_ => { });
    }
}
