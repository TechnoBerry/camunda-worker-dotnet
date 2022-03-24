using System.Linq;
using Bogus;
using Camunda.Worker.Client;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class LegacyFetchAndLockRequestProviderTests
{
    [Fact]
    public void GetRequest_ShouldReturnsRequest()
    {
        // Arrange
        var fetchAndLockOptions = new Faker<FetchAndLockOptions>()
            .RuleFor(o => o.WorkerId, f => f.Lorem.Word())
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

        var sut = new LegacyFetchAndLockRequestProvider(
            topicsProviderMock.Object,
            Options.Create(fetchAndLockOptions)
        );

        // Act
        var request = sut.GetRequest();

        // Assert
        Assert.Same(topics, request.Topics);
        Assert.Equal(fetchAndLockOptions.WorkerId, request.WorkerId);
        Assert.Equal(fetchAndLockOptions.MaxTasks, request.MaxTasks);
        Assert.Equal(fetchAndLockOptions.AsyncResponseTimeout, request.AsyncResponseTimeout);
        Assert.Equal(fetchAndLockOptions.UsePriority, request.UsePriority);
    }
}
