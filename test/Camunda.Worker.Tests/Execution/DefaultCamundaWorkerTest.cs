using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest : IDisposable
    {
        private readonly Mock<IHandler> _handlerMock = new();
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<IExternalTaskSelector> _selectorMock = new();
        private readonly ServiceProvider _serviceProvider;
        private readonly DefaultCamundaWorker _worker;

        public DefaultCamundaWorkerTest()
        {
            _serviceProvider = new ServiceCollection().BuildServiceProvider();
            _worker = new DefaultCamundaWorker(
                _selectorMock.Object,
                _clientMock.Object,
                _serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                new WorkerHandlerDescriptor(_handlerMock.Object.HandleAsync)
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

            _handlerMock
                .Setup(executor => executor.HandleAsync(It.IsAny<IExternalTaskContext>()))
                .Returns(Task.CompletedTask);

            // Act
            await _worker.RunAsync(cts.Token);

            // Assert
            _selectorMock.VerifyAll();
            _handlerMock.Verify(
                executor => executor.HandleAsync(It.IsAny<IExternalTaskContext>()),
                Times.Exactly(numberOfExternalTasks)
            );
        }

        public interface IHandler
        {
            Task HandleAsync(IExternalTaskContext context);
        }
    }
}
