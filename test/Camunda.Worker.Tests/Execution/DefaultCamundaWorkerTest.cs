using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest
    {
        private readonly Mock<IExternalTaskRouter> _routerMock = new Mock<IExternalTaskRouter>();
        private readonly Mock<IExternalTaskSelector> _selectorMock = new Mock<IExternalTaskSelector>();
        private readonly Mock<IContextFactory> _contextFactoryMock = new Mock<IContextFactory>();
        private readonly DefaultCamundaWorker _worker;

        public DefaultCamundaWorkerTest()
        {
            var contextMock = new Mock<IExternalTaskContext>();
            _contextFactoryMock.Setup(factory => factory.MakeContext(It.IsAny<ExternalTask>()))
                .Returns(contextMock.Object);

            _worker = new DefaultCamundaWorker(
                _selectorMock.Object,
                _contextFactoryMock.Object,
                new PipelineDescriptor(_routerMock.Object.RouteAsync)
            );
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
                factory => factory.MakeContext(It.IsAny<ExternalTask>()),
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
