using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest : IDisposable
    {
        private readonly Mock<IExternalTaskRouter> _routerMock = new();
        private readonly Mock<IExternalTaskSelector> _selectorMock = new();
        private readonly Mock<IContextFactory> _contextFactoryMock = new();
        private readonly ServiceProvider _serviceProvider;
        private readonly DefaultCamundaWorker _worker;

        public DefaultCamundaWorkerTest()
        {
            _serviceProvider = new ServiceCollection().BuildServiceProvider();

            var contextMock = new Mock<IExternalTaskContext>();
            _contextFactoryMock.Setup(factory => factory.Create(It.IsAny<ExternalTask>(), It.IsAny<IServiceProvider>()))
                .Returns(contextMock.Object);

            _worker = new DefaultCamundaWorker(
                _selectorMock.Object,
                _contextFactoryMock.Object,
                _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                new WorkerHandlerDescriptor(_routerMock.Object.RouteAsync)
            );
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task TestRun(int numberOfExternalTasks)
        {
            // Arrange
            var externalTasks = new Faker<ExternalTask>()
                .CustomInstantiator(faker => new ExternalTask(
                    faker.Random.Guid().ToString(),
                    faker.Random.Word(),
                    faker.Random.Word())
                )
                .GenerateLazy(numberOfExternalTasks)
                .ToList();

            var cts = new CancellationTokenSource();

            _selectorMock
                .Setup(selector => selector.SelectAsync(It.IsAny<CancellationToken>()))
                .Callback(cts.Cancel)
                .ReturnsAsync(externalTasks)
                .Verifiable();

            _routerMock
                .Setup(executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()))
                .Returns(Task.CompletedTask);

            // Act
            await _worker.Run(cts.Token);

            // Assert
            _selectorMock.VerifyAll();
            _contextFactoryMock.Verify(
                factory => factory.Create(It.IsAny<ExternalTask>(), It.IsAny<IServiceProvider>()),
                Times.Exactly(numberOfExternalTasks)
            );
            _routerMock.Verify(
                executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()),
                Times.Exactly(numberOfExternalTasks)
            );
        }
    }
}
