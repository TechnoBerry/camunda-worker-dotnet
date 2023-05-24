using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Endpoints;

internal sealed class EndpointsCollection : IEndpointsCollection
{
    private readonly List<Endpoint> _endpoints;

    public EndpointsCollection(IEnumerable<Endpoint> endpoints)
    {
        _endpoints = endpoints.ToList();
    }

    public IEnumerable<Endpoint> GetEndpoints(WorkerIdString workerId)
    {
        return _endpoints.Where(e => e.WorkerId == workerId);
    }
}
