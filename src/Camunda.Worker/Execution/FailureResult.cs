// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

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
    }
}
