using System;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public abstract class ExternalTaskHandler : IExternalTaskHandler
    {
        public virtual async Task HandleAsync(IExternalTaskContext context)
        {
            IExecutionResult executionResult;
            try
            {
                executionResult = await Process(context.Task);
            }
            catch (Exception e)
            {
                executionResult = new FailureResult(e);
            }

            await executionResult.ExecuteResultAsync(context);
        }

        public abstract Task<IExecutionResult> Process(ExternalTask externalTask);
    }
}
