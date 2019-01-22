// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker.Execution
{
    public class DefaultHandlerFactoryProvider : IHandlerFactoryProvider
    {
        private readonly IReadOnlyDictionary<string, HandlerDescriptor> _descriptors;

        public DefaultHandlerFactoryProvider(IEnumerable<HandlerDescriptor> descriptors)
        {
            _descriptors = descriptors
                .ToDictionary(descriptor => descriptor.TopicName);
        }

        public HandlerFactory GetHandlerFactory(string topicName)
        {
            if (_descriptors.TryGetValue(topicName, out var descriptor))
            {
                return descriptor.Factory;
            }

            throw new ArgumentException("Unknown topic name", nameof(topicName));
        }
    }
}
