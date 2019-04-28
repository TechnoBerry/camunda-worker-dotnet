using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class StaticTopicsProviderTest
    {
        [Fact]
        public void TestGetTopics()
        {
            var descriptors = GetDescriptors().ToList();

            var topicsProvider = new StaticTopicsProvider(descriptors);

            var topics = topicsProvider.GetTopics().ToList();

            Assert.Equal(2, topics.Count);

            Assert.Equal(descriptors[0].Metadata.TopicNames[0], topics[0].TopicName);
            Assert.Null(topics[0].Variables);

            Assert.Equal(descriptors[1].Metadata.TopicNames[0], topics[1].TopicName);
            Assert.NotNull(topics[1].Variables);
            Assert.True(topics[1].LocalVariables);
            Assert.Equal(descriptors[1].LockDuration, topics[1].LockDuration);
        }

        private static IEnumerable<HandlerDescriptor> GetDescriptors()
        {
            IExternalTaskHandler Factory(IServiceProvider provider) => null;
            var descriptors = new[]
            {
                new HandlerDescriptor(Factory, new HandlerMetadata(new[] {"topic1"})),
                new HandlerDescriptor(Factory, new HandlerMetadata(new[] {"test2"}, 10_000)
                {
                    Variables = new[] {"X"},
                    LocalVariables = true
                })
            };
            return descriptors;
        }
    }
}
