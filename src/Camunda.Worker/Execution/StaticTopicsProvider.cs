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
            return descriptor.TopicNames
                .Select(topicName => new FetchAndLockRequest.Topic(topicName, descriptor.LockDuration)
                {
                    LocalVariables = descriptor.LocalVariables,
                    Variables = descriptor.Variables
                });
        }

        public IEnumerable<FetchAndLockRequest.Topic> GetTopics()
        {
            return _topics;
        }
    }
}
