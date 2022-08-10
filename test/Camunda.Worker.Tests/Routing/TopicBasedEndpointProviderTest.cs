using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Execution;
using Xunit;

namespace Camunda.Worker.Routing;

public class TopicBasedEndpointProviderTest
{
    [Fact]
    public void TestGetKnownEndpoint()
    {
        var workerId = new Faker().Lorem.Word();

        var provider = new TopicBasedEndpointProvider(workerId, new[]
        {
            new Endpoint(
                _ => Task.CompletedTask,
                new HandlerMetadata(new[] {"topic1"}),
                workerId
            )
        });

        var endpoint = provider.GetEndpoint(new ExternalTask("test", "test", "topic1"));
        Assert.NotNull(endpoint);
    }

    [Fact]
    public void TestGetUnknownEndpoint()
    {
        var provider = new TopicBasedEndpointProvider(new Faker().Lorem.Word(), Enumerable.Empty<Endpoint>());
        var endpoint = provider.GetEndpoint(new ExternalTask("test", "test", "topic1"));

        Assert.Null(endpoint);
    }
}
