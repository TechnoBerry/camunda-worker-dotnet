using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Extensions;

namespace Camunda.Worker
{
    public sealed class CompleteResult : IExecutionResult
    {
        public CompleteResult()
        {
        }

        [Obsolete("Use constructor without arguments instead")]
        public CompleteResult(
            IDictionary<string, Variable>? variables = null,
            IDictionary<string, Variable>? localVariables = null
        )
        {
            Variables = variables;
            LocalVariables = localVariables;
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
                context.LogWarning<CompleteResult>(
                    "Failed completion of task {TaskId}. Reason: {Reason}",
                    context.Task.Id, e.Message
                );
                await context.ReportFailureAsync(e.ErrorType, e.ErrorMessage);
            }
        }
    }
}
