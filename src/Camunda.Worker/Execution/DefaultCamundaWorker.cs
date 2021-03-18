using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskSelector _selector;
        private readonly IContextFactory _contextFactory;
        private readonly PipelineDescriptor _pipelineDescriptor;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(
            IExternalTaskSelector selector,
            IContextFactory contextFactory,
            PipelineDescriptor pipelineDescriptor,
            ILogger<DefaultCamundaWorker>? logger = null
        )
        {
            _selector = Guard.NotNull(selector, nameof(selector));
            _contextFactory = Guard.NotNull(contextFactory, nameof(contextFactory));
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
            using var context = _contextFactory.MakeContext(externalTask);

            try
            {
                await _pipelineDescriptor.ExternalTaskDelegate(context);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed execution of task {Id}", context.Task.Id);
            }
        }
    }
}
