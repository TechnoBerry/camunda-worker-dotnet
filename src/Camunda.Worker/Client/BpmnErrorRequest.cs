// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace Camunda.Worker.Client
{
    public class BpmnErrorRequest
    {
        public BpmnErrorRequest(string workerId, string errorCode)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            ErrorCode = Guard.NotNull(errorCode, nameof(errorCode));
        }

        public string WorkerId { get; }
        public string ErrorCode { get; }
        public string ErrorMessage { get; set; }
        public IDictionary<string, Variable> Variables { get; set; }
    }
}
