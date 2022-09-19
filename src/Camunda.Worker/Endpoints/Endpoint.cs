namespace Camunda.Worker.Endpoints;

public sealed class Endpoint
{
    public Endpoint(ExternalTaskDelegate handlerDelegate, EndpointMetadata metadata, WorkerIdString workerId)
    {
        WorkerId = workerId;
        HandlerDelegate = Guard.NotNull(handlerDelegate, nameof(handlerDelegate));
        Metadata = Guard.NotNull(metadata, nameof(metadata));
    }

    public ExternalTaskDelegate HandlerDelegate { get; }

    public EndpointMetadata Metadata { get; }

    public WorkerIdString WorkerId { get; }
}
