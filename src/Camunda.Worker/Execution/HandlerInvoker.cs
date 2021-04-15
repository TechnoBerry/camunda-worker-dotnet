using System;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution
{
    public class HandlerInvoker
    {
        private readonly IExternalTaskHandler _handler;
        private readonly IExternalTaskContext _context;

        public HandlerInvoker(IExternalTaskHandler handler, IExternalTaskContext context)
        {
            _handler = Guard.NotNull(handler, nameof(handler));
            _context = Guard.NotNull(context, nameof(context));
        }

        public async Task InvokeAsync()
        {
            IExecutionResult executionResult;
            try
            {
                executionResult = await _handler.HandleAsync(_context.Task);
            }
            catch (Exception e)
            {
                executionResult = new FailureResult(e);
            }

            await executionResult.ExecuteResultAsync(_context);
        }
    }
}
