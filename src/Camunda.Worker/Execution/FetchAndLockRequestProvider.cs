using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Client;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution;

internal class FetchAndLockRequestProvider : IFetchAndLockRequestProvider
{
    private readonly WorkerIdString _workerId;
    private readonly FetchAndLockOptions _options;
    private readonly HandlerDescriptor[] _handlerDescriptors;

    internal FetchAndLockRequestProvider(
        WorkerIdString workerId,
        IOptionsMonitor<FetchAndLockOptions> options,
        IEnumerable<HandlerDescriptor> handlerDescriptors
    )
    {
        _workerId = workerId;
        _options = options.Get(workerId.Value);
        _handlerDescriptors = handlerDescriptors
            .Where(d => d.WorkerId == _workerId)
            .ToArray();
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
        var topics = new List<FetchAndLockRequest.Topic>(_handlerDescriptors.Length);

        foreach (var descriptor in _handlerDescriptors)
        {
            foreach (var topicName in descriptor.Metadata.TopicNames)
            {
                topics.Add(MakeTopicRequest(descriptor.Metadata, topicName));
            }
        }

        return topics;
    }

    private static FetchAndLockRequest.Topic MakeTopicRequest(HandlerMetadata metadata, string topicName) =>
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
