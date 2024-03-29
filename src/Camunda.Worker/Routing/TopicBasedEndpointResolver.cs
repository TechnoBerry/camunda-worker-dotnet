using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Endpoints;

namespace Camunda.Worker.Routing;

public class TopicBasedEndpointResolver : IEndpointResolver
{
    private readonly IReadOnlyDictionary<string, Endpoint> _endpoints;

    public TopicBasedEndpointResolver(WorkerIdString workerId, IEndpointsCollection endpointsCollection)
    {
        _endpoints = endpointsCollection.GetEndpoints(workerId)
            .SelectMany(endpoint => endpoint.Metadata.TopicNames
                .Select(topicName => new
                {
                    TopicName = topicName,
                    Endpoint = endpoint
                })
            )
            .ToDictionary(pair => pair.TopicName, pair => pair.Endpoint);
    }

    public Endpoint? Resolve(ExternalTask externalTask)
    {
        Guard.NotNull(externalTask, nameof(externalTask));
        return _endpoints.GetValueOrDefault(externalTask.TopicName);
    }
}
