// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class FailureResult : IExecutionResult
    {
        public string ErrorMessage { get; }
        public string ErrorDetails { get; }

        public FailureResult(Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorDetails = ex.StackTrace;
        }

        public async Task ExecuteResult(ExternalTaskContext context, CancellationToken cancellationToken)
        {
            var client = context.CamundaApiClient;
            var taskId = context.ExternalTask.Id;
            var workerId = context.ExternalTask.WorkerId;

            await client.ReportFailure(taskId, new ReportFailureRequest
            {
                WorkerId = workerId,
                ErrorMessage = ErrorMessage,
                ErrorDetails = ErrorDetails
            }, cancellationToken);
        }
    }
}
