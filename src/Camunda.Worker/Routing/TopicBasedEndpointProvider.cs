using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Execution;

namespace Camunda.Worker.Routing;

public class TopicBasedEndpointProvider : IEndpointProvider
{
    private readonly IReadOnlyDictionary<string, Endpoint> _endpoints;

    public TopicBasedEndpointProvider(WorkerIdString workerId, IEnumerable<Endpoint> endpoints)
    {
        _endpoints = endpoints
            .Where(endpoint => endpoint.WorkerId == workerId)
            .SelectMany(endpoint => endpoint.Metadata.TopicNames
                .Select(topicName => new
                {
                    TopicName = topicName,
                    Endpoint = endpoint
                })
            )
            .ToDictionary(pair => pair.TopicName, pair => pair.Endpoint);
    }

    public Endpoint? GetEndpoint(ExternalTask externalTask)
    {
        Guard.NotNull(externalTask, nameof(externalTask));
        return _endpoints.GetValueOrDefault(externalTask.TopicName);
    }
}
