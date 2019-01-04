// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using Camunda.Worker.Execution;

namespace Camunda.Worker
{
    public sealed class HandlerDescriptor
    {
        public HandlerDescriptor(string topicName, HandlerFactory factory)
        {
            TopicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public string TopicName { get; }
        public HandlerFactory Factory { get; }
        public bool LocalVariables { get; set; }
        public IEnumerable<string> Variables { get; set; }
    }
}
