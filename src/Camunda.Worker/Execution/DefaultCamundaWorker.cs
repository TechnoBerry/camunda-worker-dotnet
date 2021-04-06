using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskSelector _selector;
        private readonly IContextFactory _contextFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PipelineDescriptor _pipelineDescriptor;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(
            IExternalTaskSelector selector,
            IContextFactory contextFactory,
            IServiceScopeFactory serviceScopeFactory,
            PipelineDescriptor pipelineDescriptor,
            ILogger<DefaultCamundaWorker>? logger = null
        )
        {
            _selector = Guard.NotNull(selector, nameof(selector));
            _contextFactory = Guard.NotNull(contextFactory, nameof(contextFactory));
            _serviceScopeFactory = Guard.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            _pipelineDescriptor = Guard.NotNull(pipelineDescriptor, nameof(pipelineDescriptor));
            _logger = logger ?? NullLogger<DefaultCamundaWorker>.Instance;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTasks = await _selector.SelectAsync(cancellationToken);

                var executableTasks = externalTasks
                    .Select(ProcessExternalTask)
                    .ToList();

                await Task.WhenAll(executableTasks);
            }
        }

        private async Task ProcessExternalTask(ExternalTask externalTask)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = _contextFactory.Create(externalTask, scope.ServiceProvider);

            try
            {
                await _pipelineDescriptor.ExternalTaskDelegate(context);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed execution of task {Id}", externalTask.Id);
            }
        }
    }
}
