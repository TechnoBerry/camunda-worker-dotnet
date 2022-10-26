using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Variables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker;

public sealed class CompleteResult : IExecutionResult
{
    public CompleteResult()
    {
    }

    public Dictionary<string, VariableBase>? Variables { get; set; }

    public Dictionary<string, VariableBase>? LocalVariables { get; set; }

    public async Task ExecuteResultAsync(IExternalTaskContext context)
    {
        var externalTask = context.Task;
        var client = context.Client;

        try
        {
            await client.CompleteAsync(externalTask.Id, new CompleteRequest(externalTask.WorkerId)
            {
                Variables = Variables,
                LocalVariables = LocalVariables,
            });
        }
        catch (ClientException e) when (e.StatusCode == HttpStatusCode.InternalServerError)
        {
            var logger = context.ServiceProvider.GetService<ILogger<CompleteResult>>()
                         ?? NullLogger<CompleteResult>.Instance;
            Log.Result_FailedCompletion(logger, externalTask.Id, e.Message, e);
            await client.ReportFailureAsync(externalTask.Id, new ReportFailureRequest(externalTask.WorkerId)
            {
                ErrorMessage = e.ErrorType,
                ErrorDetails = e.ErrorMessage,
            });
        }
    }
}
