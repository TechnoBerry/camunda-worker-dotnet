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

        public FailureResult(Exception ex) : this(ex.Message, ex.StackTrace)
        {
        }

        public FailureResult(string errorMessage, string errorDetails = null)
        {
            ErrorMessage = Guard.NotNull(errorMessage, nameof(errorMessage));
            ErrorDetails = errorDetails;
        }

        public Task ExecuteResultAsync(IExternalTaskContext context)
        {
            return context.ReportFailureAsync(ErrorMessage, ErrorDetails, Retries, RetryTimeout);
        }
    }
}
