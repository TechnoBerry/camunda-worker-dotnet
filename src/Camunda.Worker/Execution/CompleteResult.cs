// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public class CompleteResult : IExecutionResult
    {
        public IDictionary<string, Variable> Variables { get; }

        public CompleteResult(IDictionary<string, Variable> variables)
        {
            Variables = variables ?? new Dictionary<string, Variable>();
        }
    }
}
