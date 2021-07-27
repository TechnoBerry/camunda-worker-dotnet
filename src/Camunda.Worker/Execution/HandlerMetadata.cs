using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public class HandlerMetadata
    {
        public HandlerMetadata(IReadOnlyList<string> topicNames, int lockDuration = Constants.MinimumLockDuration)
        {
            TopicNames = Guard.NotNull(topicNames, nameof(topicNames));
            LockDuration = Guard.GreaterThanOrEqual(lockDuration, Constants.MinimumLockDuration, nameof(lockDuration));
        }

        public IReadOnlyList<string> TopicNames { get; }

        public int LockDuration { get; }

        public bool LocalVariables { get; set; }

        /// <summary>Determines whether serializable variable values (typically variables that store custom Java objects) should be deserialized on server side (default false).</summary>
        public bool DeserializeValues { get; set; }

        /// <summary>Determines whether custom extension properties defined in the BPMN activity of the external task (e.g. via the Extensions tab in the Camunda modeler) should be included in the response. Default: false.</summary>
        public bool IncludeExtensionProperties { get; set; }
        
        public IReadOnlyList<string>? Variables { get; set; }

        public IReadOnlyList<string>? ProcessDefinitionIds { get; set; }

        public IReadOnlyList<string>? ProcessDefinitionKeys { get; set; }

        public IReadOnlyList<string>? TenantIds { get; set; }
    }
}
