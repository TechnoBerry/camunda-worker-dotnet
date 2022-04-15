using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class TopicBasedEndpointProviderTest
{
    [Fact]
    public void TestGetKnownEndpointDelegate()
    {
        Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

        var workerId = new WorkerIdString("testWorker");
        var provider = new TopicBasedEndpointProvider(new[]
        {
            new HandlerEndpoint(FakeHandlerDelegate, new HandlerMetadata(new[] {"topic1"}), workerId)
        });

        var handlerDelegate = provider.GetEndpointDelegate(new ExternalTask("test", "test", "topic1"));
        Assert.NotNull(handlerDelegate);
    }

    [Fact]
    public void TestGetUnknownEndpointDelegate()
    {
        var provider = new TopicBasedEndpointProvider(Enumerable.Empty<HandlerEndpoint>());
        Assert.Throws<ArgumentException>(
            () => provider.GetEndpointDelegate(new ExternalTask("test", "test", "topic1"))
        );
    }
}
