using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public sealed class HandlerDescriptor
    {
        public HandlerDescriptor(HandlerFactory factory, HandlerMetadata metadata)
        {
            Factory = Guard.NotNull(factory, nameof(factory));
            Metadata = Guard.NotNull(metadata, nameof(metadata));
        }

        public HandlerFactory Factory { get; }

        public HandlerMetadata Metadata { get; }

        public IEnumerable<string> TopicNames => Metadata.TopicNames;

        public bool LocalVariables => Metadata.LocalVariables;

        public IEnumerable<string> Variables => Metadata.Variables;

        public int LockDuration => Metadata.LockDuration;
    }
}
