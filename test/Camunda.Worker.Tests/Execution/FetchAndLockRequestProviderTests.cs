using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class FetchAndLockRequestProviderTests
{
    [Fact]
    public void GetRequest_ShouldReturnsRequest()
    {
        // Arrange
        var workerId = new WorkerIdString(new Faker().Lorem.Word());
        var fetchAndLockOptions = new Faker<FetchAndLockOptions>()
            .RuleFor(o => o.MaxTasks, f => f.Random.Int(1, 10))
            .RuleFor(o => o.AsyncResponseTimeout, f => f.Random.Int(100, 10000))
            .RuleFor(o => o.UsePriority, f => f.Random.Bool())
            .Generate();

        var endpoints = GetEndpoints(workerId);
        var endpointsCollectionMock = new Mock<IEndpointsCollection>();
        endpointsCollectionMock.Setup(e => e.GetEndpoints(workerId))
            .Returns(endpoints);

        var sut = new FetchAndLockRequestProvider(
            workerId,
            CreateOptions(workerId.Value, fetchAndLockOptions),
            endpointsCollectionMock.Object
        );

        // Act
        var request = sut.GetRequest();

        // Assert
        Assert.NotNull(request.Topics);
        Assert.Collection(request.Topics, endpoints
            .SelectMany(endpoint => endpoint.Metadata.TopicNames.Select(topicName => (topicName, endpoint.Metadata)))
            .Select(pair => new Action<FetchAndLockRequest.Topic>(topic =>
            {
                Assert.Equal(pair.topicName, topic.TopicName);
                Assert.Equal(pair.Metadata.LockDuration, topic.LockDuration);
            }))
            .ToArray()
        );
        Assert.Equal(workerId.Value, request.WorkerId);
        Assert.Equal(fetchAndLockOptions.MaxTasks, request.MaxTasks);
        Assert.Equal(fetchAndLockOptions.AsyncResponseTimeout, request.AsyncResponseTimeout);
        Assert.Equal(fetchAndLockOptions.UsePriority, request.UsePriority);
    }

    private static IOptionsMonitor<T> CreateOptions<T>(string optionsKey, T value)
    {
        var optionsMonitorMock = new Mock<IOptionsMonitor<T>>();
        optionsMonitorMock
            .Setup(o => o.Get(optionsKey))
            .Returns(value);

        return optionsMonitorMock.Object;
    }

    private static Endpoint[] GetEndpoints(WorkerIdString workerId)
    {
        Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;
        return new[]
        {
            new Endpoint(FakeHandlerDelegate, new EndpointMetadata(new[] {"topic1"}), workerId),
            new Endpoint(FakeHandlerDelegate, new EndpointMetadata(new[] {"test2"}, 10_000)
            {
                Variables = new[] {"X"},
                LocalVariables = true
            }, workerId)
        };
    }
}
