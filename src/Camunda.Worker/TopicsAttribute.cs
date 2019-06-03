using System;
using System.Collections.Generic;
using System.Linq;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TopicsAttribute : Attribute
    {
        private int _lockDuration = Constants.MinimumLockDuration;

        public TopicsAttribute(params string[] topicNames)
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
