#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Execution
{
    public class TopicBasedFactoryProvider : IHandlerFactoryProvider
    {
        private readonly IReadOnlyDictionary<string, HandlerFactory> _factories;

        public TopicBasedFactoryProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _factories = descriptors
                .SelectMany(descriptor => descriptor.Metadata.TopicNames
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
