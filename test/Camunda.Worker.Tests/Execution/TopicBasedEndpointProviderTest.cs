using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class TopicBasedEndpointProviderTest
{
    [Fact]
    public void TestGetKnownEndpointDelegate()
    {
        Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

        var provider = new TopicBasedEndpointProvider(new[]
        {
            new HandlerDescriptor(FakeHandlerDelegate, new HandlerMetadata(new[] {"topic1"}), new Faker().Lorem.Word())
        });

        var handlerDelegate = provider.GetEndpointDelegate(new ExternalTask("test", "test", "topic1"));
        Assert.NotNull(handlerDelegate);
    }

    [Fact]
    public void TestGetUnknownEndpointDelegate()
    {
        var provider = new TopicBasedEndpointProvider(Enumerable.Empty<HandlerDescriptor>());
        Assert.Throws<ArgumentException>(
            () => provider.GetEndpointDelegate(new ExternalTask("test", "test", "topic1"))
        );
    }
}
