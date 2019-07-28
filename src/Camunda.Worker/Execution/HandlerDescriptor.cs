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
    }
}
