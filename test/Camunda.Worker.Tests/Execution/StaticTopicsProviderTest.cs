using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Xunit;

namespace Camunda.Worker.Execution;

public class StaticTopicsProviderTest
{
    [Fact]
    public void TestGetTopics()
    {
        var descriptors = GetDescriptors();

        var topicsProvider = new StaticTopicsProvider(descriptors);

        var topics = topicsProvider.GetTopics().ToList();

        Assert.Equal(2, topics.Count);

        Assert.Equal(descriptors[0].Metadata.TopicNames[0], topics[0].TopicName);
        Assert.Null(topics[0].Variables);

        Assert.Equal(descriptors[1].Metadata.TopicNames[0], topics[1].TopicName);
        Assert.NotNull(topics[1].Variables);
        Assert.True(topics[1].LocalVariables);
        Assert.Equal(descriptors[1].Metadata.LockDuration, topics[1].LockDuration);
    }

    private static HandlerDescriptor[] GetDescriptors()
    {
        var workerId = new Faker().Lorem.Word();
        Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;
        return new[]
        {
            new HandlerDescriptor(FakeHandlerDelegate, new HandlerMetadata(new[] {"topic1"}), workerId),
            new HandlerDescriptor(FakeHandlerDelegate, new HandlerMetadata(new[] {"test2"}, 10_000)
            {
                Variables = new[] {"X"},
                LocalVariables = true
            }, workerId)
        };
    }
}
