using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Execution
{
    public class TopicBasedHandlerDelegateProvider : IHandlerDelegateProvider
    {
        private readonly IReadOnlyDictionary<string, HandlerFactory> _factories;

        public TopicBasedHandlerDelegateProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _factories = descriptors
                .SelectMany(descriptor => descriptor.TopicNames
                    .Select(topicName => (topicName, descriptor.Factory))
                )
                .ToDictionary(pair => pair.topicName, pair => pair.Factory);
        }

        public HandlerFactory GetHandlerFactory(ExternalTask externalTask)
        {
            Guard.NotNull(externalTask, nameof(externalTask));

            var topicName = externalTask.TopicName;

            return GetHandlerFactoryByTopicName(topicName);
        }

        protected virtual HandlerFactory GetHandlerFactoryByTopicName(string topicName)
        {
            if (_factories.TryGetValue(topicName, out var factory))
            {
                return factory;
            }

            throw new ArgumentException("Unknown topic name", nameof(topicName));
        }
    }
}
