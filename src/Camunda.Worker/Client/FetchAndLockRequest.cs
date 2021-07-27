using System.Collections.Generic;

namespace Camunda.Worker.Client
{
    public class FetchAndLockRequest
    {
        public FetchAndLockRequest(string workerId, int maxTasks = 1)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            MaxTasks = Guard.GreaterThanOrEqual(maxTasks, 1, nameof(maxTasks));
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

        public IReadOnlyCollection<Topic>? Topics { get; set; }

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

            public IReadOnlyCollection<string>? Variables { get; set; }

            public string? BusinessKey { get; set; }

            /// <summary>Determines whether serializable variable values (typically variables that store custom Java objects) should be deserialized on server side (default false).</summary>
            public bool DeserializeValues { get; set; }

            /// <summary>Determines whether custom extension properties defined in the BPMN activity of the external task (e.g. via the Extensions tab in the Camunda modeler) should be included in the response. Default: false.</summary>
            public bool IncludeExtensionProperties { get; set; }

            public string? ProcessDefinitionId { get; set; }

            public IReadOnlyCollection<string>? ProcessDefinitionIdIn { get; set; }

            public string? ProcessDefinitionKey { get; set; }

            public IReadOnlyCollection<string>? ProcessDefinitionKeyIn { get; set; }

            public bool? WithoutTenantId { get; set; }

            public IReadOnlyCollection<string>? TenantIdIn { get; set; }
        }
    }
}
