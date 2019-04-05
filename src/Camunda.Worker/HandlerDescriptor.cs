#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;

namespace Camunda.Worker
{
    public sealed class HandlerDescriptor
    {
        private int _lockDuration = 5_000;

        public HandlerDescriptor(string topicName, HandlerFactory factory)
        {
            TopicName = Guard.NotNull(topicName, nameof(topicName));
            Factory = Guard.NotNull(factory, nameof(factory));
        }

        public string TopicName { get; }

        public HandlerFactory Factory { get; }

        public bool LocalVariables { get; set; }

        public IEnumerable<string> Variables { get; set; }

        public int LockDuration
        {
            get => _lockDuration;
            set => _lockDuration = Guard.GreaterThanOrEqual(value, 5_000, nameof(LockDuration));
        }
    }
}
