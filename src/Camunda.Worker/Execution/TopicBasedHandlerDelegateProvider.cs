using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Execution
{
    public class TopicBasedHandlerDelegateProvider : IHandlerDelegateProvider
    {
        private readonly IReadOnlyDictionary<string, ExternalTaskDelegate> _handlerDelegates;

        public TopicBasedHandlerDelegateProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _handlerDelegates = descriptors
                .SelectMany(descriptor => descriptor.TopicNames
                    .Select(topicName => (topicName, Factory: descriptor.HandlerDelegate))
                )
                .ToDictionary(pair => pair.topicName, pair => pair.Factory);
        }

        public ExternalTaskDelegate GetHandlerDelegate(ExternalTask externalTask)
        {
            Guard.NotNull(externalTask, nameof(externalTask));

            var handlerDelegate = GetHandlerDelegateByTopicName(externalTask.TopicName);
            return handlerDelegate;
        }

        protected virtual ExternalTaskDelegate GetHandlerDelegateByTopicName(string topicName)
        {
            if (_handlerDelegates.TryGetValue(topicName, out var handlerDelegate))
            {
                return handlerDelegate;
            }

            throw new ArgumentException("Unknown topic name", nameof(topicName));
        }
    }
}
