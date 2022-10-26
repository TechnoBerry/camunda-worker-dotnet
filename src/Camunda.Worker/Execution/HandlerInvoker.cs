using System;
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
        _logger.LogInvoker_StartedProcessing(_context.Task.Id);
        IExecutionResult executionResult;
        try
        {
            executionResult = await _handler.HandleAsync(_context.Task, _context.ProcessingAborted);
        }
        catch (Exception e) when (!_context.ProcessingAborted.IsCancellationRequested)
        {
            _logger.LogInvoker_FailedProcessing(_context.Task.Id, e);
            executionResult = new FailureResult(e);
        }

        await executionResult.ExecuteResultAsync(_context);
        _logger.LogInvoker_FinishedProcessing(_context.Task.Id);
    }
}
