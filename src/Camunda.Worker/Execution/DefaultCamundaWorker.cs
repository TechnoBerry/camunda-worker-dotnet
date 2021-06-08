using System;
using System.Diagnostics.CodeAnalysis;
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
        private readonly WorkerHandlerDescriptor _workerHandlerDescriptor;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(
            IExternalTaskSelector selector,
            IContextFactory contextFactory,
            IServiceScopeFactory serviceScopeFactory,
            WorkerHandlerDescriptor workerHandlerDescriptor,
            ILogger<DefaultCamundaWorker>? logger = null
        )
        {
            _selector = Guard.NotNull(selector, nameof(selector));
            _contextFactory = Guard.NotNull(contextFactory, nameof(contextFactory));
            _serviceScopeFactory = Guard.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            _workerHandlerDescriptor = Guard.NotNull(workerHandlerDescriptor, nameof(workerHandlerDescriptor));
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
                await _workerHandlerDescriptor.ExternalTaskDelegate(context);
            }
            catch (Exception e)
            {
                Log.FailedExecution(_logger, externalTask.Id, e);
            }
        }

        [ExcludeFromCodeCoverage]
        private static class Log
        {
            private static readonly Action<ILogger, string, Exception?> _failedExecution =
                LoggerMessage.Define<string>(
                    LogLevel.Warning,
                    new EventId(0),
                    "Failed execution of task {Id}"
                );

            public static void FailedExecution(ILogger logger, string externalTaskId, Exception e) =>
                _failedExecution(logger, externalTaskId, e);
        }
    }
}
