using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public class HandlerInvoker
    {
        private readonly IExternalTaskHandler _handler;
        private readonly IExternalTaskContext _context;
        private readonly ILogger<HandlerInvoker> _logger;

        public HandlerInvoker(IExternalTaskHandler handler, IExternalTaskContext context)
        {
            _handler = Guard.NotNull(handler, nameof(handler));
            _context = Guard.NotNull(context, nameof(context));
            _logger = context.ServiceProvider.GetService<ILogger<HandlerInvoker>>() ??
                      NullLogger<HandlerInvoker>.Instance;
        }

        public async Task InvokeAsync()
        {
            Log.StartedProcessing(_logger, _context.Task.Id);
            IExecutionResult executionResult;
            try
            {
                executionResult = await _handler.HandleAsync(_context.Task, default);
            }
            catch (Exception e)
            {
                executionResult = new FailureResult(e);
            }

            await executionResult.ExecuteResultAsync(_context);
            Log.FinishedProcessing(_logger, _context.Task.Id);
        }

        [ExcludeFromCodeCoverage]
        private static class Log
        {
            private static readonly Action<ILogger, string, Exception?> _startedProcessing =
                LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(0),
                    "Started processing of task {TaskId}"
                );

            private static readonly Action<ILogger, string, Exception?> _finishedProcessing =
                LoggerMessage.Define<string>(
                    LogLevel.Debug,
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
