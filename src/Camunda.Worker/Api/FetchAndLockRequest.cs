// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using Newtonsoft.Json;

namespace Camunda.Worker.Api
{
    public class FetchAndLockRequest
    {
        /// <summary>
        /// The id of the worker on which behalf tasks are fetched
        /// </summary>
        public string WorkerId { get; set; }

        /// <summary>
        /// The maximum number of tasks to return
        /// </summary>
        public int MaxTasks { get; set; }

        /// <summary>
        /// A value, which indicates whether the task should be fetched based on its priority or arbitrarily
        /// </summary>
        public bool UsePriority { get; set; }

        /// <summary>
        /// The long polling timeout in milliseconds
        /// </summary>
        public int AsyncResponseTimeout { get; set; }

        public IEnumerable<Topic> Topics { get; set; }

        public class Topic
        {
            /// <summary>
            /// The topic's name
            /// </summary>
            public string TopicName { get; set; }

            /// <summary>
            /// The duration to lock the external tasks for in milliseconds
            /// </summary>
            public int LockDuration { get; set; }

            public bool LocalVariables { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public IEnumerable<string> Variables { get; set; }
        }
    }
}
