using System.Linq;
using Bogus;
using Camunda.Worker.Client;
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

        var topics = new Faker<FetchAndLockRequest.Topic>()
            .CustomInstantiator(f => new FetchAndLockRequest.Topic(f.Random.Hash(), f.Random.Int(1000, 10000)))
            .GenerateLazy(5)
            .ToList();
        var topicsProviderMock = new Mock<ITopicsProvider>();
        topicsProviderMock.Setup(p => p.GetTopics()).Returns(topics);

        var sut = new FetchAndLockRequestProvider(
            workerId,
            topicsProviderMock.Object,
            CreateOptions(workerId.Value, fetchAndLockOptions)
        );

        // Act
        var request = sut.GetRequest();

        // Assert
        Assert.Same(topics, request.Topics);
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
}
