#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task ExecuteResultAsync(IExternalTaskContext context)
        {
            return context.ReportBpmnErrorAsync(ErrorCode, ErrorMessage, Variables);
        }
    }
}
