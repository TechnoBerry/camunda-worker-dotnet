using Camunda.Worker.Endpoints;

namespace Camunda.Worker.Routing;

public interface IEndpointResolver
{
    Endpoint? Resolve(ExternalTask externalTask);
}
