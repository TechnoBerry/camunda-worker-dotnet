using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IEndpointProvider _endpointProvider;
        private readonly ILogger<ExternalTaskRouter> _logger;

        public ExternalTaskRouter(
            IEndpointProvider endpointProvider,
            ILogger<ExternalTaskRouter>? logger = null
        )
        {
            _endpointProvider = Guard.NotNull(endpointProvider, nameof(endpointProvider));
            _logger = logger ?? NullLogger<ExternalTaskRouter>.Instance;
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var externalTaskDelegate = _endpointProvider.GetEndpointDelegate(context.Task);
            var externalTask = context.Task;

            Log.StartedProcessing(_logger, externalTask.Id);

            await externalTaskDelegate(context);

            Log.FinishedProcessing(_logger, externalTask.Id);
        }

        private static class Log
        {
            private static readonly Action<ILogger, string, Exception?> _startedProcessing =
                LoggerMessage.Define<string>(
                    LogLevel.Information,
                    new EventId(0),
                    "Started processing of task {TaskId}"
                );

            private static readonly Action<ILogger, string, Exception?> _finishedProcessing =
                LoggerMessage.Define<string>(
                    LogLevel.Information,
                    new EventId(0),
                    "Finished processing of task {TaskId}"
                );

            public static void StartedProcessing(ILogger logger, string taskId) =>
                _startedProcessing(logger, taskId, null);

            public static void FinishedProcessing(ILogger logger, string taskId) =>
                _finishedProcessing(logger, taskId, null);
        }
    }
}
