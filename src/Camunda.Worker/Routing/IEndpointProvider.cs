using Camunda.Worker.Execution;

namespace Camunda.Worker.Routing;

public interface IEndpointProvider
{
    Endpoint? GetEndpoint(ExternalTask externalTask);
}
