using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class TopicBasedHandlerDelegateProviderTest
    {
        [Fact]
        public void TestGetKnownHandlerDelegate()
        {
            Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

            var provider = new TopicBasedHandlerDelegateProvider(new[]
            {
                new HandlerDescriptor(FakeHandlerDelegate, new HandlerMetadata(new[] {"topic1"}))
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
