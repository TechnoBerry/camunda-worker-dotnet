// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class BpmnErrorResult : IExecutionResult
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public IDictionary<string, Variable> Variables { get; }

        public BpmnErrorResult(string errorCode, string errorMessage, IDictionary<string, Variable> variables = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Variables = variables ?? new Dictionary<string, Variable>();
        }

        public async Task ExecuteResult(ExternalTaskContext context, CancellationToken cancellationToken)
        {
            var client = context.CamundaApiClient;
            var taskId = context.ExternalTask.Id;
            var workerId = context.ExternalTask.WorkerId;

            await client.ReportBpmnError(taskId, new BpmnErrorRequest
            {
                WorkerId = workerId,
                ErrorCode = ErrorCode,
                ErrorMessage = ErrorMessage,
                Variables = Variables
            }, cancellationToken);
        }
    }
}
