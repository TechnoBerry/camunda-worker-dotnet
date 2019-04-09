using System.Collections.Generic;

namespace Camunda.Worker.Extensions
{
    internal class HandlerMetadata
    {
        public IReadOnlyList<string> TopicNames { get; set; }
        public int LockDuration { get; set; }
        public bool LocalVariables { get; set; }
        public IReadOnlyList<string> Variables { get; set; }
    }
}
