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
        private readonly IExternalTaskClient _client;

        public ExternalTaskContext(ExternalTask task, IServiceScope scope)
        {
            Task = Guard.NotNull(task, nameof(task));
            _scope = Guard.NotNull(scope, nameof(scope));
            _client = _scope.ServiceProvider.GetRequiredService<IExternalTaskClient>();
        }

        public ExternalTask Task { get; }

        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        public bool Completed { get; private set; }

        public async Task ExtendLockAsync(int newDuration)
        {
            ThrowIfDisposed();

            var request = new ExtendLockRequest(Task.WorkerId, newDuration);
            await _client.ExtendLock(Task.Id, request);
        }

        public async Task CompleteAsync(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var request = new CompleteRequest(Task.WorkerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };
            await _client.Complete(Task.Id, request);

            Completed = true;
        }

        public async Task ReportFailureAsync(string errorMessage, string errorDetails,
            int? retries = default,
            int? retryTimeout = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var request = new ReportFailureRequest(Task.WorkerId)
            {
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                Retries = retries,
                RetryTimeout = retryTimeout
            };
            await _client.ReportFailure(Task.Id, request);

            Completed = true;
        }

        public async Task ReportBpmnErrorAsync(string errorCode, string errorMessage,
            IDictionary<string, Variable> variables = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var request = new BpmnErrorRequest(Task.WorkerId, errorCode, errorMessage)
            {
                Variables = variables
            };
            await _client.ReportBpmnError(Task.Id, request);

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
