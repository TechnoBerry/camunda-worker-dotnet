#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HandlerTopicsAttribute : Attribute
    {
        private int _lockDuration = Constants.MinimumLockDuration;

        public HandlerTopicsAttribute(params string[] topicNames)
        {
            Guard.NotNull(topicNames, nameof(topicNames));

            TopicNames = topicNames.ToList();
        }

        public IReadOnlyList<string> TopicNames { get; }

        public int LockDuration
        {
            get => _lockDuration;
            set => _lockDuration = Guard.GreaterThanOrEqual(value, Constants.MinimumLockDuration, nameof(LockDuration));
        }
    }
}
