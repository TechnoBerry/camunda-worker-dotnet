// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultTopicsProviderTest
    {
        [Fact]
        public void TestGetTopics()
        {
            IExternalTaskHandler Factory(IServiceProvider provider) => null;

            var descriptors = new[]
            {
                new HandlerDescriptor("test1", Factory),
                new HandlerDescriptor("test2", Factory)
                {
                    Variables = new[] {"X"},
                    LockDuration = 10_000,
                    LocalVariables = true
                }
            };

            var topicsProvider = new DefaultTopicsProvider(descriptors);

            var topics = topicsProvider.GetTopics().ToList();

            Assert.Equal(2, topics.Count);

            Assert.Equal("test1", topics[0].TopicName);
            Assert.Null(topics[0].Variables);

            Assert.Equal("test2", topics[1].TopicName);
            Assert.NotNull(topics[1].Variables);
            Assert.True(topics[1].LocalVariables);
            Assert.Equal(10_000, topics[1].LockDuration);
        }
    }
}
