using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker
{
    public sealed class CompleteResult : IExecutionResult
    {
        public CompleteResult()
        {
        }

        public IDictionary<string, Variable>? Variables { get; set; }

        public IDictionary<string, Variable>? LocalVariables { get; set; }

        public async Task ExecuteResultAsync(IExternalTaskContext context)
        {
            try
            {
                await context.CompleteAsync(Variables, LocalVariables);
            }
            catch (ClientException e) when (e.StatusCode == HttpStatusCode.InternalServerError)
            {
                var logger = context.ServiceProvider.GetService<ILogger<CompleteResult>>();
                logger?.LogWarning(e, "Failed completion of task {TaskId}. Reason: {Reason}",
                    context.Task.Id, e.Message
                );
                await context.ReportFailureAsync(e.ErrorType, e.ErrorMessage);
            }
        }
    }
}
