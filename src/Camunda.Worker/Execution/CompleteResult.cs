// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class CompleteResult : IExecutionResult
    {
        public IDictionary<string, Variable> Variables { get; }

        public CompleteResult(IDictionary<string, Variable> variables)
        {
            Variables = variables ?? new Dictionary<string, Variable>();
        }

        public async Task ExecuteResult(ExternalTaskContext context, CancellationToken cancellationToken)
        {
            var client = context.CamundaApiClient;
            var taskId = context.ExternalTask.Id;
            var workerId = context.ExternalTask.WorkerId;

            await client.Complete(taskId, new CompleteRequest
            {
                WorkerId = workerId,
                Variables = Variables
            }, cancellationToken);
        }
    }
}
