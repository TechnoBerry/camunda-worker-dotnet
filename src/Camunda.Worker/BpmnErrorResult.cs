// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public sealed class BpmnErrorResult : IExecutionResult
    {
        public BpmnErrorResult(string errorCode, string errorMessage, IDictionary<string, Variable> variables = null)
        {
            ErrorCode = Guard.NotNull(errorCode, nameof(errorCode));
            ErrorMessage = Guard.NotNull(errorMessage, nameof(errorMessage));
            Variables = variables ?? new Dictionary<string, Variable>();
        }

        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public IDictionary<string, Variable> Variables { get; }

        public async Task ExecuteResult(ExternalTaskContext context)
        {
            var client = context.ExternalTaskCamundaClient;
            var taskId = context.ExternalTask.Id;
            var workerId = context.ExternalTask.WorkerId;

            await client.ReportBpmnError(taskId, new BpmnErrorRequest(workerId, ErrorCode)
            {
                ErrorMessage = ErrorMessage,
                Variables = Variables
            });
        }
    }
}
