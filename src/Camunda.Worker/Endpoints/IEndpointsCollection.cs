using System.Collections.Generic;

namespace Camunda.Worker.Endpoints;

public interface IEndpointsCollection
{
    IEnumerable<Endpoint> GetEndpoints(WorkerIdString workerId);
}
