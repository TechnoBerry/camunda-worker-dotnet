// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class DefaultTopicsProvider : ITopicsProvider
    {
        private readonly IReadOnlyList<FetchAndLockRequest.Topic> _topics;

        public DefaultTopicsProvider(IEnumerable<HandlerDescriptor> handlerDescriptors)
        {
            _topics = ExtractTopics(handlerDescriptors).ToList();
        }

        private static IEnumerable<FetchAndLockRequest.Topic> ExtractTopics(IEnumerable<HandlerDescriptor> descriptors)
        {
            return descriptors
                .Select(descriptor => new FetchAndLockRequest.Topic
                {
                    TopicName = descriptor.TopicName,
                    LockDuration = descriptor.LockDuration,
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
