using System;
using System.Linq;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class TopicBasedHandlerDelegateProviderTest
    {
        [Fact]
        public void TestGetKnownHandlerDelegate()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();

            var provider = new TopicBasedHandlerDelegateProvider(new[]
            {
                new HandlerDescriptor(p => handlerMock.Object, new HandlerMetadata(new[] {"topic1"}))
            });

            var handlerDelegate = provider.GetHandlerDelegate(new ExternalTask("test", "test", "topic1"));
            Assert.NotNull(handlerDelegate);
        }

        [Fact]
        public void TestGetUnknownHandlerDelegate()
        {
            var provider = new TopicBasedHandlerDelegateProvider(Enumerable.Empty<HandlerDescriptor>());
            Assert.Throws<ArgumentException>(
                () => provider.GetHandlerDelegate(new ExternalTask("test", "test", "topic1"))
            );
        }
    }
}
