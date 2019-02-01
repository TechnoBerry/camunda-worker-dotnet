// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class StaticTopicsProvider : ITopicsProvider
    {
        private readonly IReadOnlyList<FetchAndLockRequest.Topic> _topics;

        public StaticTopicsProvider(IEnumerable<HandlerDescriptor> handlerDescriptors)
        {
            _topics = handlerDescriptors.Select(ConvertDescriptorToTopic).ToList();
        }

        protected virtual FetchAndLockRequest.Topic ConvertDescriptorToTopic(HandlerDescriptor descriptor)
        {
            return new FetchAndLockRequest.Topic
            {
                TopicName = descriptor.TopicName,
                LockDuration = descriptor.LockDuration,
                LocalVariables = descriptor.LocalVariables,
                Variables = descriptor.Variables
            };
        }

        public IEnumerable<FetchAndLockRequest.Topic> GetTopics()
        {
            return _topics;
        }
    }
}
