#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HandlerTopicAttribute : Attribute
    {
        private int _lockDuration = 5_000;

        public HandlerTopicAttribute(string topicName)
        {
            TopicName = Guard.NotNull(topicName, nameof(topicName));
        }

        public string TopicName { get; }

        public int LockDuration
        {
            get => _lockDuration;
            set
            {
                if (value < 5_000)
                {
                    throw new ArgumentException("'LockDuration' must be greater than or equal to 5000");
                }

                _lockDuration = value;
            }
        }
    }
}
