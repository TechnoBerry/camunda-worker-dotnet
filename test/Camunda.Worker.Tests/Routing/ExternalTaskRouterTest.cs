using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Routing;

public class ExternalTaskRouterTest
{
    private readonly Mock<IExternalTaskContext> _contextMock = new();
    private readonly Mock<IEndpointProvider> _endpointProviderMock = new();

    public ExternalTaskRouterTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(_endpointProviderMock.Object)
            .BuildServiceProvider();

        _contextMock.SetupGet(context => context.ServiceProvider).Returns(serviceProvider);
        _contextMock.SetupGet(context => context.Task).Returns(new ExternalTask("1", "testWorker", "testTopic"));
    }

    [Fact]
    public async Task TestRouteAsync()
    {
        // Arrange
        var calls = new List<IExternalTaskContext>();

        Task ExternalTaskDelegate(IExternalTaskContext context)
        {
            calls.Add(context);
            return Task.CompletedTask;
        }

        _endpointProviderMock.Setup(factory => factory.GetEndpointDelegate(It.IsAny<ExternalTask>()))
            .Returns(ExternalTaskDelegate);

        // Act
        await ExternalTaskRouter.RouteAsync(_contextMock.Object);

        // Assert
        Assert.Single(calls);
    }
}
