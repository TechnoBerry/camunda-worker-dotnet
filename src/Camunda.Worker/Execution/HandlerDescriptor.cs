namespace Camunda.Worker.Execution;

public sealed class HandlerDescriptor
{
    public HandlerDescriptor(ExternalTaskDelegate handlerDelegate, HandlerMetadata metadata, WorkerIdString workerId)
    {
        WorkerId = workerId;
        HandlerDelegate = Guard.NotNull(handlerDelegate, nameof(handlerDelegate));
        Metadata = Guard.NotNull(metadata, nameof(metadata));
    }

    public ExternalTaskDelegate HandlerDelegate { get; }

    public HandlerMetadata Metadata { get; }

    public WorkerIdString WorkerId { get; }
}
