using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker
{
    public sealed class BpmnErrorResult : IExecutionResult
    {
        public BpmnErrorResult(string errorCode, string errorMessage, IDictionary<string, Variable>? variables = null)
        {
            ErrorCode = Guard.NotEmptyAndNotNull(errorCode, nameof(errorCode));
            ErrorMessage = Guard.NotEmptyAndNotNull(errorMessage, nameof(errorMessage));
            Variables = variables;
        }

        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public IDictionary<string, Variable>? Variables { get; }

        public async Task ExecuteResultAsync(IExternalTaskContext context)
        {
            try
            {
                await context.ReportBpmnErrorAsync(ErrorCode, ErrorMessage, Variables);
            }
            catch (ClientException e) when (e.StatusCode == HttpStatusCode.InternalServerError)
            {
                var logger = context.ServiceProvider.GetService<ILogger<BpmnErrorResult>>();
                logger?.LogWarning(e, "Failed completion of task {TaskId}. Reason: {Reason}",
                    context.Task.Id, e.Message
                );
                await context.ReportFailureAsync(e.ErrorType, e.ErrorMessage);
            }
        }
    }
}
