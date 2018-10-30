// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace Camunda.Worker.Core
{
    public class ExecutionResult
    {
        public ExecutionResult(IDictionary<string, Variable> variables)
        {
            Variables = variables ?? throw new ArgumentNullException(nameof(variables));
            Exception = null;
        }

        public ExecutionResult(Exception exception)
        {
            Variables = null;
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public IDictionary<string, Variable> Variables { get; }
        public Exception Exception { get; }
        public bool Success => Exception == null;
    }
}
