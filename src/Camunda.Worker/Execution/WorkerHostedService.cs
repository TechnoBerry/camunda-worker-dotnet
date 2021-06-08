using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Camunda.Worker.Execution
{
    public sealed class WorkerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _numberOfWorkers;

        public WorkerHostedService(IServiceProvider serviceProvider, int numberOfWorkers)
        {
            _serviceProvider = Guard.NotNull(serviceProvider, nameof(serviceProvider));
            _numberOfWorkers =
                Guard.GreaterThanOrEqual(numberOfWorkers, Constants.MinimumParallelExecutors, nameof(numberOfWorkers));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var activeTasks = Enumerable.Range(0, _numberOfWorkers)
                .Select(_ => _serviceProvider.GetRequiredService<ICamundaWorker>())
                .Select(worker => worker.RunAsync(stoppingToken))
                .ToList();
            return Task.WhenAll(activeTasks);
        }
    }
}
