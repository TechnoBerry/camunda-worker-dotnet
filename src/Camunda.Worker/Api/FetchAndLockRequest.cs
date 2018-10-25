// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace Camunda.Worker.Api
{
    public class FetchAndLockRequest
    {
        public string WorkerId { get; set; }
        public int MaxTasks { get; set; }
        public IEnumerable<Topic> Topics { get; set; }

        public class Topic
        {
            public string TopicName { get; set; }
            public int LockDuration { get; set; }
        }
    }
}
