// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HandlerTopicAttribute : Attribute
    {
        public HandlerTopicAttribute(string topicName)
        {
            TopicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
        }

        public string TopicName { get; }
    }
}
