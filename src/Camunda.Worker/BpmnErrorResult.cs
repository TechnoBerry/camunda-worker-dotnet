using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Variables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker;

public sealed class BpmnErrorResult : IExecutionResult
{
    public BpmnErrorResult(string errorCode, string errorMessage, Dictionary<string, VariableBase>? variables = null)
    {
        ErrorCode = Guard.NotEmptyAndNotNull(errorCode, nameof(errorCode));
        ErrorMessage = Guard.NotEmptyAndNotNull(errorMessage, nameof(errorMessage));
        Variables = variables;
    }

    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public Dictionary<string, VariableBase>? Variables { get; }

    public async Task ExecuteResultAsync(IExternalTaskContext context)
    {
        var externalTask = context.Task;
        var client = context.Client;

        try
        {
            await client.ReportBpmnErrorAsync(
                externalTask.Id,
                new BpmnErrorRequest(externalTask.WorkerId, ErrorCode, ErrorMessage)
                {
                    Variables = Variables,
                }
            );
        }
        catch (ClientException e) when (e.StatusCode == HttpStatusCode.InternalServerError)
        {
            var logger = context.ServiceProvider.GetService<ILogger<BpmnErrorResult>>();
            logger?.LogWarning(e, "Failed completion of task {TaskId}. Reason: {Reason}",
                externalTask.Id, e.Message
            );
            await client.ReportFailureAsync(externalTask.Id, new ReportFailureRequest(externalTask.WorkerId)
            {
                ErrorMessage = e.ErrorType,
                ErrorDetails = e.ErrorMessage,
            });
        }
    }
}
