using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskRouterTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new();
        private readonly Mock<IEndpointProvider> _endpointProviderMock = new();
        private readonly ExternalTaskRouter _router;

        public ExternalTaskRouterTest()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(_endpointProviderMock.Object)
                .BuildServiceProvider();

            _contextMock.SetupGet(context => context.ServiceProvider).Returns(serviceProvider);
            _contextMock.SetupGet(context => context.Task).Returns(new ExternalTask("1", "testWorker", "testTopic"));
            _router = new ExternalTaskRouter();
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
            await _router.RouteAsync(_contextMock.Object);

            // Assert
            Assert.Single(calls);
        }
    }
}
