using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class WorkerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CamundaWorkerOptions _options;

        public WorkerHostedService(IServiceProvider serviceProvider, IOptions<CamundaWorkerOptions> options)
        {
            _serviceProvider = Guard.NotNull(serviceProvider, nameof(serviceProvider));
            _options = Guard.NotNull(options, nameof(options)).Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var activeTasks = Enumerable.Range(0, _options.WorkerCount)
                .Select(_ => _serviceProvider.GetRequiredService<ICamundaWorker>())
                .Select(worker => worker.Run(stoppingToken))
                .ToList();
            return Task.WhenAll(activeTasks);
        }
    }
}
