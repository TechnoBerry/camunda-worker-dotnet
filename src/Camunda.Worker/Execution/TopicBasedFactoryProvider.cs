// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Execution
{
    public class TopicBasedFactoryProvider : IHandlerFactoryProvider
    {
        private readonly IReadOnlyDictionary<string, HandlerDescriptor> _descriptors;

        public TopicBasedFactoryProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _descriptors = descriptors
                .ToDictionary(descriptor => descriptor.TopicName);
        }

        public HandlerFactory GetHandlerFactory(ExternalTask externalTask)
        {
            Guard.NotNull(externalTask, nameof(externalTask));

            var topicName = externalTask.TopicName;

            return GetHandlerFactoryByTopicName(topicName);
        }

        protected virtual HandlerFactory GetHandlerFactoryByTopicName(string topicName)
        {
            if (_descriptors.TryGetValue(topicName, out var descriptor))
            {
                return descriptor.Factory;
            }

            throw new ArgumentException("Unknown topic name", nameof(topicName));
        }
    }
}
