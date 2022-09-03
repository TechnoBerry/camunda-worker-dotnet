using Camunda.Worker.Execution;

namespace Camunda.Worker.Routing;

public interface IEndpointResolver
{
    Endpoint? Resolve(ExternalTask externalTask);
}
