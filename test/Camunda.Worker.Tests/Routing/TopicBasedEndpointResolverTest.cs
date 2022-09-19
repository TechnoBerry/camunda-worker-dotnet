using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Endpoints;
using Moq;
using Xunit;

namespace Camunda.Worker.Routing;

public class TopicBasedEndpointResolverTest
{
    [Fact]
    public void TestResolveKnownEndpoint()
    {
        var workerId = new Faker().Lorem.Word();
        var endpointsCollectionMock = new Mock<IEndpointsCollection>();
        endpointsCollectionMock.Setup(e => e.GetEndpoints(workerId))
            .Returns(new[]
            {
                new Endpoint(
                    _ => Task.CompletedTask,
                    new EndpointMetadata(new[] { "topic1" }),
                    workerId
                )
            });

        var provider = new TopicBasedEndpointResolver(workerId, endpointsCollectionMock.Object);

        var endpoint = provider.Resolve(new ExternalTask("test", "test", "topic1"));
        Assert.NotNull(endpoint);
    }

    [Fact]
    public void TestResolveUnknownEndpoint()
    {
        var endpointsCollectionMock = new Mock<IEndpointsCollection>();
        endpointsCollectionMock.Setup(e => e.GetEndpoints(It.IsAny<WorkerIdString>()))
            .Returns(Enumerable.Empty<Endpoint>());

        var provider = new TopicBasedEndpointResolver(new Faker().Lorem.Word(), endpointsCollectionMock.Object);
        var endpoint = provider.Resolve(new ExternalTask("test", "test", "topic1"));

        Assert.Null(endpoint);
    }
}
