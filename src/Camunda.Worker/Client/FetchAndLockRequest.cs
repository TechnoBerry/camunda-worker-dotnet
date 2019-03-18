// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using Newtonsoft.Json;

namespace Camunda.Worker.Client
{
    public class FetchAndLockRequest
    {
        public FetchAndLockRequest(string workerId, int maxTasks = 1)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            MaxTasks = maxTasks;
        }

        /// <summary>
        /// The id of the worker on which behalf tasks are fetched
        /// </summary>
        public string WorkerId { get; }

        /// <summary>
        /// The maximum number of tasks to return
        /// </summary>
        public int MaxTasks { get; }

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
            public Topic(string topicName, int lockDuration)
            {
                TopicName = Guard.NotNull(topicName, nameof(topicName));
                LockDuration = lockDuration;
            }

            /// <summary>
            /// The topic's name
            /// </summary>
            public string TopicName { get; }

            /// <summary>
            /// The duration to lock the external tasks for in milliseconds
            /// </summary>
            public int LockDuration { get; }

            /// <summary>
            /// If <c>true</c> only local variables will be fetched
            /// </summary>
            public bool LocalVariables { get; set; }

            public IEnumerable<string> Variables { get; set; }

            public string BusinessKey { get; set; }

            public string ProcessDefinitionId { get; set; }

            public IEnumerable<string> ProcessDefinitionIdIn { get; set; }

            public string ProcessDefinitionKey { get; set; }

            public IEnumerable<string> ProcessDefinitionKeyIn { get; set; }

            public bool WithoutTenantId { get; set; }

            public IEnumerable<string> TenantIdIn { get; set; }
        }
    }
}
