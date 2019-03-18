#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;

namespace Camunda.Worker.Client
{
    public class CompleteRequest
    {
        public CompleteRequest(string workerId)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
        }

        public string WorkerId { get; }
        public IDictionary<string, Variable> Variables { get; set; }
        public IDictionary<string, Variable> LocalVariables { get; set; }
    }
}
