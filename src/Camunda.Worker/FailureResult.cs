using System;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public sealed class FailureResult : IExecutionResult
    {
        public string ErrorMessage { get; }
        public string? ErrorDetails { get; }
        public int? Retries { get; set; }
        public int? RetryTimeout { get; set; }

        public FailureResult(Exception ex) : this(ex.Message, ex.StackTrace)
        {
        }

        public FailureResult(string errorMessage, string? errorDetails = null)
        {
            ErrorMessage = Guard.NotNull(errorMessage, nameof(errorMessage));
            ErrorDetails = errorDetails;
        }

        public async Task ExecuteResultAsync(IExternalTaskContext context)
        {
            var externalTask = context.Task;
            var client = context.Client;

            await client.ReportFailureAsync(externalTask.Id, new ReportFailureRequest(externalTask.WorkerId)
            {
                ErrorMessage = ErrorMessage,
                ErrorDetails = ErrorDetails,
                Retries = Retries,
                RetryTimeout = RetryTimeout
            });
        }
    }
}
