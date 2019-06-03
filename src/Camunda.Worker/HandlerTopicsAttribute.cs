using System;

namespace Camunda.Worker
{
    [Obsolete("Will be removed after `0.8.0` release use \"TopicsAttribute\" instead")]
    [AttributeUsage(AttributeTargets.Class)]
    public class HandlerTopicsAttribute : TopicsAttribute
    {
        public HandlerTopicsAttribute(params string[] topicNames) : base(topicNames)
        {
        }
    }
}
