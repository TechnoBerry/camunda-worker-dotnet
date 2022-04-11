namespace Camunda.Worker.Execution;

public sealed class HandlerDescriptor
{
    internal HandlerDescriptor(ExternalTaskDelegate handlerDelegate, HandlerMetadata metadata, WorkerIdString workerId)
    {
        HandlerDelegate = Guard.NotNull(handlerDelegate, nameof(handlerDelegate));
        Metadata = Guard.NotNull(metadata, nameof(metadata));
        WorkerId = Guard.NotNull(workerId, nameof(workerId));
    }

    public ExternalTaskDelegate HandlerDelegate { get; }

    public HandlerMetadata Metadata { get; }

    public WorkerIdString WorkerId { get; }
}
