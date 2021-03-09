using System;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public abstract class ExternalTaskHandler : IExternalTaskHandler
    {
        public async Task HandleAsync(IExternalTaskContext context)
        {
            IExecutionResult executionResult;
            try
            {
                executionResult = await HandleAsync(context.Task);
            }
            catch (Exception e)
            {
                executionResult = new FailureResult(e);
            }

            await executionResult.ExecuteResultAsync(context);
        }

        public abstract Task<IExecutionResult> HandleAsync(ExternalTask externalTask);
    }
}
