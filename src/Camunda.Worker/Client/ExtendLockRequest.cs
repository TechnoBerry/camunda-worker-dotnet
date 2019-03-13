// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Camunda.Worker.Client
{
    public class ExtendLockRequest
    {
        public ExtendLockRequest(string workerId, int newDuration)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            NewDuration = newDuration;
        }

        public string WorkerId { get; }
        public int NewDuration { get; }
    }
}
