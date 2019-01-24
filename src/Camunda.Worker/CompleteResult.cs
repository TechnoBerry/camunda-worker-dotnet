// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public sealed class CompleteResult : IExecutionResult
    {
        public CompleteResult(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = null)
        {
            Variables = variables ?? new Dictionary<string, Variable>();
            LocalVariables = localVariables ?? new Dictionary<string, Variable>();
        }

        public IDictionary<string, Variable> Variables { get; }

        public IDictionary<string, Variable> LocalVariables { get; }

        public async Task ExecuteResult(ExternalTaskContext context, CancellationToken cancellationToken)
        {
            var client = context.ExternalTaskCamundaClient;
            var taskId = context.ExternalTask.Id;
            var workerId = context.ExternalTask.WorkerId;

            await client.Complete(taskId, new CompleteRequest
            {
                WorkerId = workerId,
                Variables = Variables,
                LocalVariables = LocalVariables
            }, cancellationToken);
        }
    }
}
