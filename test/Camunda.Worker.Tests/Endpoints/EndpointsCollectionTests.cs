using System.Threading.Tasks;
using Bogus;
using Xunit;

namespace Camunda.Worker.Endpoints;

public class EndpointsCollectionTests
{
    [Fact]
    public void GetEndpoints_Should_ReturnOnlyEndpointsWithRequestedWorkerId()
    {
        // Arrange
        var workerId1 = new WorkerIdString(new Faker().Lorem.Word());
        var workerId2 = new WorkerIdString(new Faker().Lorem.Word());

        static Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

        var worker1Endpoint = new Endpoint(FakeHandlerDelegate, new EndpointMetadata(["topic1"]), workerId1);
        var worker2Endpoint = new Endpoint(FakeHandlerDelegate, new EndpointMetadata(["test2"], 10_000)
        {
            Variables = ["X"],
            LocalVariables = true
        }, workerId2);

        var sut = new EndpointsCollection(new [] { worker1Endpoint, worker2Endpoint });

        // Act
        var result = sut.GetEndpoints(workerId1);

        // Assert
        var resultEndpoint = Assert.Single(result);
        Assert.Same(worker1Endpoint, resultEndpoint);
    }
}
