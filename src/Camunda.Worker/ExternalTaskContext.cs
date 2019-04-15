#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public sealed class ExternalTaskContext : IExternalTaskContext, IDisposable
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

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new ExtendLockRequest(workerId, newDuration);

            await _client.ExtendLock(taskId, request);
        }

        public async Task CompleteAsync(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = null)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new CompleteRequest(workerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };

            await _client.Complete(taskId, request);

            Completed = true;
        }

        public async Task ReportFailureAsync(string errorMessage, string errorDetails,
            int? retries = default,
            int? retryTimeout = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new ReportFailureRequest(workerId)
            {
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails,
                Retries = retries,
                RetryTimeout = retryTimeout
            };

            await _client.ReportFailure(taskId, request);

            Completed = true;
        }

        public async Task ReportBpmnErrorAsync(string errorCode, string errorMessage,
            IDictionary<string, Variable> variables = null)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new BpmnErrorRequest(workerId, errorCode)
            {
                ErrorMessage = errorMessage,
                Variables = variables
            };

            await _client.ReportBpmnError(taskId, request);

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
            _scope?.Dispose();
            _disposed = true;
        }
    }
}
