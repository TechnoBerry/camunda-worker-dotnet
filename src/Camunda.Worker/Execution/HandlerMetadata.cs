#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public class HandlerMetadata
    {
        public HandlerMetadata(IReadOnlyList<string> topicNames, int lockDuration = 5_000)
        {
            TopicNames = Guard.NotNull(topicNames, nameof(topicNames));
            LockDuration = Guard.GreaterThanOrEqual(lockDuration, 5_000, nameof(lockDuration));
        }

        public IReadOnlyList<string> TopicNames { get; }
        public int LockDuration { get; }
        public bool LocalVariables { get; set; }
        public IReadOnlyList<string> Variables { get; set; }
        public IReadOnlyList<string> ProcessDefinitionIds { get; set; }
        public IReadOnlyList<string> ProcessDefinitionKeys { get; set; }
    }
}
