using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public sealed class HandlerDescriptor
    {
        public HandlerDescriptor(ExternalTaskDelegate handlerDelegate, HandlerMetadata metadata)
        {
            HandlerDelegate = Guard.NotNull(handlerDelegate, nameof(handlerDelegate));
            Metadata = Guard.NotNull(metadata, nameof(metadata));
        }

        public ExternalTaskDelegate HandlerDelegate { get; }

        public HandlerMetadata Metadata { get; }

        public IEnumerable<string> TopicNames => Metadata.TopicNames;

        public bool LocalVariables => Metadata.LocalVariables;

        public IEnumerable<string> Variables => Metadata.Variables;

        public int LockDuration => Metadata.LockDuration;
    }
}
