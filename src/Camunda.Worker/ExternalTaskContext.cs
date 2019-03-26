#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public sealed class ExternalTaskContext : IExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask task, IExternalTaskCamundaClient client)
        {
            Task = Guard.NotNull(task, nameof(task));
            Client = Guard.NotNull(client, nameof(client));
        }

        public ExternalTask Task { get; }
        private IExternalTaskCamundaClient Client { get; }
        public bool Completed { get; private set; }

        public async Task CompleteAsync(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = null)
        {
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new CompleteRequest(workerId)
            {
                Variables = variables,
                LocalVariables = localVariables
            };

            await Client.Complete(taskId, request);

            Completed = true;
        }

        public async Task ReportFailureAsync(string errorMessage, string errorDetails)
        {
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new ReportFailureRequest(workerId)
            {
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails
            };

            await Client.ReportFailure(taskId, request);

            Completed = true;
        }

        public async Task ReportBpmnErrorAsync(string errorCode, string errorMessage,
            IDictionary<string, Variable> variables = null)
        {
            ThrowIfCompleted();

            var taskId = Task.Id;
            var workerId = Task.WorkerId;
            var request = new BpmnErrorRequest(workerId, errorCode)
            {
                ErrorMessage = errorMessage,
                Variables = variables
            };

            await Client.ReportBpmnError(taskId, request);

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
