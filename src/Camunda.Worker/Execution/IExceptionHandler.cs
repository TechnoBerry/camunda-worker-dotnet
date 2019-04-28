using System;

namespace Camunda.Worker.Execution
{
    public interface IExceptionHandler
    {
        bool TryTransformToResult(Exception exception, out IExecutionResult executionResult);
    }
}
