using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution;

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
        LogHelper.LogStartedProcessing(_logger, _context.Task.Id);
        IExecutionResult executionResult;
        try
        {
            executionResult = await _handler.HandleAsync(_context.Task, _context.ProcessingAborted);
        }
        catch (Exception e) when (!_context.ProcessingAborted.IsCancellationRequested)
        {
            executionResult = new FailureResult(e);
        }

        await executionResult.ExecuteResultAsync(_context);
        LogHelper.LogFinishedProcessing(_logger, _context.Task.Id);
    }

    [ExcludeFromCodeCoverage]
    private static class LogHelper
    {
        private static readonly Action<ILogger, string, Exception?> StartedProcessing =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(0),
                "Started processing of task {TaskId}"
            );

        private static readonly Action<ILogger, string, Exception?> FinishedProcessing =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(0),
                "Finished processing of task {TaskId}"
            );

        public static void LogStartedProcessing(ILogger logger, string taskId)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                StartedProcessing(logger, taskId, null);
            }
        }

        public static void LogFinishedProcessing(ILogger logger, string taskId)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                FinishedProcessing(logger, taskId, null);
            }
        }
    }
}
