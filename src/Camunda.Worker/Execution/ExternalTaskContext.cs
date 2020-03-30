using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskContext : IExternalTaskContext
    {
        private bool _disposed;
        private readonly IServiceScope _scope;

        public ExternalTaskContext(ExternalTask task, IServiceScope scope)
        {
            Task = Guard.NotNull(task, nameof(task));
            _scope = Guard.NotNull(scope, nameof(scope));
        }

        public ExternalTask Task { get; }

        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        public bool Completed { get; private set; }

        public async Task ExtendLockAsync(int newDuration)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            using var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new ExtendLockRequest(Task.WorkerId, newDuration);
            await client.ExtendLockAsync(Task.Id, request);
        }

        public async Task CompleteAsync(IDictionary<string, Variable> variables = null,
            IDictionary<string, Variable> localVariables = null)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            using var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new CompleteRequest(Task.WorkerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };
            await client.CompleteAsync(Task.Id, request);

            Completed = true;
        }

        public async Task ReportFailureAsync(string errorMessage, string errorDetails,
            int? retries = default,
            int? retryTimeout = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            using var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
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

        public async Task ReportBpmnErrorAsync(string errorCode, string errorMessage,
            IDictionary<string, Variable> variables = null)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            using var client = ServiceProvider.GetRequiredService<IExternalTaskClient>();
            var request = new BpmnErrorRequest(Task.WorkerId, errorCode, errorMessage)
            {
                Variables = variables
            };
            await client.ReportBpmnErrorAsync(Task.Id, request);

            Completed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void ThrowIfCompleted()
        {
            if (Completed)
            {
                throw new CamundaWorkerException("Unable to complete already completed task");
            }
        }


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _scope?.Dispose();
            _disposed = true;
        }
    }
}
