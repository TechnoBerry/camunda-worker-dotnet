#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using Newtonsoft.Json;

namespace Camunda.Worker
{
    public class ExternalTask
    {
        [JsonConstructor]
        public ExternalTask(string id, string workerId, string topicName)
        {
            Id = Guard.NotNull(id, nameof(id));
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            TopicName = Guard.NotNull(topicName, nameof(topicName));
        }

        public string Id { get; }
        public string WorkerId { get; }
        public string TopicName { get; }
        public string TenantId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessDefinitionKey { get; set; }
        public string ProcessInstanceId { get; set; }
        public string BusinessKey { get; set; }
        public IDictionary<string, Variable> Variables { get; set; }
    }
}
