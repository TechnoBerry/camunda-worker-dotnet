using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Endpoints;
using Xunit;

namespace Camunda.Worker.Routing;

public class TopicBasedEndpointResolverTest
{
    [Fact]
    public void TestResolveKnownEndpoint()
    {
        var workerId = new Faker().Lorem.Word();

        var provider = new TopicBasedEndpointResolver(workerId, new[]
        {
            new Endpoint(
                _ => Task.CompletedTask,
                new HandlerMetadata(new[] {"topic1"}),
                workerId
            )
        });

        var endpoint = provider.Resolve(new ExternalTask("test", "test", "topic1"));
        Assert.NotNull(endpoint);
    }

    [Fact]
    public void TestResolveUnknownEndpoint()
    {
        var provider = new TopicBasedEndpointResolver(new Faker().Lorem.Word(), Enumerable.Empty<Endpoint>());
        var endpoint = provider.Resolve(new ExternalTask("test", "test", "topic1"));

        Assert.Null(endpoint);
    }
}
