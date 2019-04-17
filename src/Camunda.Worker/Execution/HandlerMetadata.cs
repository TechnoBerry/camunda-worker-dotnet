using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    internal class HandlerMetadata
    {
        public HandlerMetadata(IReadOnlyList<string> topicNames, int lockDuration)
        {
            TopicNames = Guard.NotNull(topicNames, nameof(topicNames));
            LockDuration = Guard.GreaterThanOrEqual(lockDuration, 5_000, nameof(lockDuration));
        }

        public IReadOnlyList<string> TopicNames { get; }
        public int LockDuration { get; }
        public bool LocalVariables { get; set; }
        public IReadOnlyList<string> Variables { get; set; }
    }
}
