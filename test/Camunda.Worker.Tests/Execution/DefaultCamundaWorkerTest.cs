using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
                new PipelineDescriptor(_routerMock.Object.RouteAsync)
            );
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task TestRunWithoutTasks()
        {
            var cts = new CancellationTokenSource();

            ConfigureSelector(cts, new List<ExternalTask>());

            await _worker.Run(cts.Token);

            _routerMock.VerifyNoOtherCalls();
            _selectorMock.VerifyAll();
        }

        [Fact]
        public async Task TestRunWithTask()
        {
            var cts = new CancellationTokenSource();

            ConfigureSelector(cts, new List<ExternalTask>
            {
                new ExternalTask("test", "test", "test")
            });

            _routerMock
                .Setup(executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);

            await _worker.Run(cts.Token);

            _contextFactoryMock.Verify(
                factory => factory.Create(It.IsAny<ExternalTask>(), It.IsAny<IServiceProvider>()),
                Times.Once()
            );
            _selectorMock.VerifyAll();
            _routerMock.Verify(
                executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()),
                Times.Once()
            );
        }

        private void ConfigureSelector(CancellationTokenSource cts, IReadOnlyCollection<ExternalTask> externalTasks)
        {
            _selectorMock
                .Setup(selector => selector.SelectAsync(
                    It.IsAny<CancellationToken>()
                ))
                .Callback(cts.Cancel)
                .ReturnsAsync(externalTasks);
        }
    }
}
