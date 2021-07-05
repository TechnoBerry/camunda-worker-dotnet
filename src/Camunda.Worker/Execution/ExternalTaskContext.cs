using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskContext : IExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask task, IServiceProvider provider)
        {
            Task = Guard.NotNull(task, nameof(task));
            ServiceProvider = Guard.NotNull(provider, nameof(provider));
        }

        public ExternalTask Task { get; }

        public IServiceProvider ServiceProvider { get; }

        public bool Completed { get; private set; }

        public async Task ExtendLockAsync(int newDuration)
        {
            ThrowIfCompleted();

            var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new ExtendLockRequest(Task.WorkerId, newDuration);
            await client.ExtendLockAsync(Task.Id, request);
        }

        public async Task CompleteAsync(
            IDictionary<string, Variable>? variables = null,
            IDictionary<string, Variable>? localVariables = null
        )
        {
            ThrowIfCompleted();

            var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new CompleteRequest(Task.WorkerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };
            await client.CompleteAsync(Task.Id, request);

            Completed = true;
        }

        public async Task ReportFailureAsync(
            string? errorMessage,
            string? errorDetails,
            int? retries = default,
            int? retryTimeout = default
        )
        {
            ThrowIfCompleted();

            var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new ReportFailureRequest(Task.WorkerId)
            {
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                Retries = retries,
                RetryTimeout = retryTimeout
            };
            await client.ReportFailureAsync(Task.Id, request);

            Completed = true;
        }

        public async Task ReportBpmnErrorAsync(
            string errorCode,
            string errorMessage,
            IDictionary<string, Variable>? variables = null
        )
        {
            ThrowIfCompleted();

            var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new BpmnErrorRequest(Task.WorkerId, errorCode, errorMessage)
            {
                Variables = variables
            };
            await client.ReportBpmnErrorAsync(Task.Id, request);

            Completed = true;
        }

        private void ThrowIfCompleted()
        {
            if (Completed)
            {
                throw new CamundaWorkerException("Unable to complete already completed task");
            }
        }
    }
}
