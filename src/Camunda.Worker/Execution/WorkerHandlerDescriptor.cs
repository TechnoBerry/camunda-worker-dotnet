namespace Camunda.Worker.Execution;

public class WorkerHandlerDescriptor
{
    public WorkerHandlerDescriptor(ExternalTaskDelegate externalTaskDelegate)
    {
        ExternalTaskDelegate = Guard.NotNull(externalTaskDelegate, nameof(externalTaskDelegate));
    }

    public ExternalTaskDelegate ExternalTaskDelegate { get; }
}
