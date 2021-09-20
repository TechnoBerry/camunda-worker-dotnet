using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskContext : IExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask task, IExternalTaskClient client, IServiceProvider provider)
        {
            Task = Guard.NotNull(task, nameof(task));
            Client = Guard.NotNull(client, nameof(client));
            ServiceProvider = Guard.NotNull(provider, nameof(provider));
        }

        public ExternalTask Task { get; }

        public IExternalTaskClient Client { get; }

        public IServiceProvider ServiceProvider { get; }

        public async Task ExtendLockAsync(int newDuration)
        {
            var request = new ExtendLockRequest(Task.WorkerId, newDuration);
            await Client.ExtendLockAsync(Task.Id, request);
        }

        public async Task CompleteAsync(
            IDictionary<string, Variable>? variables = null,
            IDictionary<string, Variable>? localVariables = null
        )
        {
            var request = new CompleteRequest(Task.WorkerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };
            await Client.CompleteAsync(Task.Id, request);
        }

        public async Task ReportFailureAsync(
            string? errorMessage,
            string? errorDetails,
            int? retries = default,
            int? retryTimeout = default
        )
        {
            var request = new ReportFailureRequest(Task.WorkerId)
            {
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                Retries = retries,
                RetryTimeout = retryTimeout
            };
            await Client.ReportFailureAsync(Task.Id, request);
        }

        public async Task ReportBpmnErrorAsync(
            string errorCode,
            string errorMessage,
            IDictionary<string, Variable>? variables = null
        )
        {
            var request = new BpmnErrorRequest(Task.WorkerId, errorCode, errorMessage)
            {
                Variables = variables
            };
            await Client.ReportBpmnErrorAsync(Task.Id, request);
        }
    }
}
