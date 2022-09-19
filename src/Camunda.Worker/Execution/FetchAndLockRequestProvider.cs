using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution;

public sealed class FetchAndLockRequestProvider : IFetchAndLockRequestProvider
{
    private readonly WorkerIdString _workerId;
    private readonly FetchAndLockOptions _options;
    private readonly Endpoint[] _endpoints;

    public FetchAndLockRequestProvider(
        WorkerIdString workerId,
        IOptionsMonitor<FetchAndLockOptions> options,
        IEndpointsCollection endpointsCollection
    )
    {
        _workerId = workerId;
        _options = options.Get(workerId.Value);
        _endpoints = endpointsCollection.GetEndpoints(workerId).ToArray();
    }

    public FetchAndLockRequest GetRequest()
    {
        var topics = GetTopics();

        var fetchAndLockRequest = new FetchAndLockRequest(_workerId.Value, _options.MaxTasks)
        {
            UsePriority = _options.UsePriority,
            AsyncResponseTimeout = _options.AsyncResponseTimeout,
            Topics = topics
        };

        return fetchAndLockRequest;
    }

    private List<FetchAndLockRequest.Topic> GetTopics()
    {
        var topics = new List<FetchAndLockRequest.Topic>(_endpoints.Length);

        foreach (var endpoint in _endpoints)
        {
            foreach (var topicName in endpoint.Metadata.TopicNames)
            {
                topics.Add(MakeTopicRequest(endpoint.Metadata, topicName));
            }
        }

        return topics;
    }

    private static FetchAndLockRequest.Topic MakeTopicRequest(EndpointMetadata metadata, string topicName) =>
        new(topicName, metadata.LockDuration)
        {
            LocalVariables = metadata.LocalVariables,
            Variables = metadata.Variables,
            ProcessDefinitionIdIn = metadata.ProcessDefinitionIds,
            ProcessDefinitionKeyIn = metadata.ProcessDefinitionKeys,
            ProcessVariables = metadata.ProcessVariables,
            TenantIdIn = metadata.TenantIds,
            DeserializeValues = metadata.DeserializeValues,
            IncludeExtensionProperties = metadata.IncludeExtensionProperties,
        };
}
