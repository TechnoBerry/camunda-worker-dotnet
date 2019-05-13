using System;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public sealed class FailureResult : IExecutionResult
    {
        public string ErrorMessage { get; }
        public string ErrorDetails { get; }
        public int? Retries { get; set; }
        public int? RetryTimeout { get; set; }

        public FailureResult(Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorDetails = ex.StackTrace;
        }

        public FailureResult(string errorMessage, string errorDetails)
        {
            ErrorMessage = Guard.NotNull(errorMessage, nameof(errorMessage));
            ErrorDetails = Guard.NotNull(errorDetails, nameof(errorDetails));
        }

        public Task ExecuteResultAsync(IExternalTaskContext context)
        {
            return context.ReportFailureAsync(ErrorMessage, ErrorDetails, Retries, RetryTimeout);
        }
    }
}
