#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public sealed class FailureResult : IExecutionResult
    {
        public string ErrorMessage { get; }
        public string ErrorDetails { get; }

        public FailureResult(Exception ex)
        {
            ErrorMessage = ex.Message;
            ErrorDetails = ex.StackTrace;
        }

        public FailureResult(string errorMessage, string errorDetails)
        {
            ErrorMessage = Guard.NotNull(errorMessage, nameof(errorMessage));
            ErrorDetails = Guard.NotNull(errorDetails, nameof(errorDetails));
        }

        public Task ExecuteResultAsync(IExternalTaskContext context)
        {
            return context.ReportFailureAsync(ErrorMessage, ErrorDetails);
        }
    }
}
