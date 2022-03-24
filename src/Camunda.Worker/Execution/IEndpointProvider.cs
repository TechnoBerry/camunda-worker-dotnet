namespace Camunda.Worker.Execution;

public interface IEndpointProvider
{
    ExternalTaskDelegate GetEndpointDelegate(ExternalTask externalTask);
}
