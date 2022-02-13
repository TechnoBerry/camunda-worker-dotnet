using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public sealed class StaticTopicsProvider : ITopicsProvider
    {
        private readonly IReadOnlyList<FetchAndLockRequest.Topic> _topics;

        public StaticTopicsProvider(IEnumerable<HandlerDescriptor> handlerDescriptors)
        {
            _topics = handlerDescriptors.SelectMany(ConvertDescriptorToTopic).ToList();
        }

        private static IEnumerable<FetchAndLockRequest.Topic> ConvertDescriptorToTopic(HandlerDescriptor descriptor)
        {
            return descriptor.Metadata.TopicNames
                .Select(topicName => MakeTopicRequest(descriptor.Metadata, topicName));
        }

        private static FetchAndLockRequest.Topic MakeTopicRequest(HandlerMetadata metadata, string topicName) =>
            new FetchAndLockRequest.Topic(topicName, metadata.LockDuration)
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

        public IReadOnlyCollection<FetchAndLockRequest.Topic> GetTopics()
        {
            return _topics;
        }
    }
}
