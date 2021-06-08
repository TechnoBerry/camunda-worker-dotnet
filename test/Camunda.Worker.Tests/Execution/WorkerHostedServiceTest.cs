using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class WorkerHostedServiceTest
    {
        private readonly Mock<ICamundaWorker> _workerMock = new();
        private readonly Mock<IServiceProvider> _providerMock = new();

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async Task TestRunWorkerOnStart(int numberOfWorkers)
        {
            _providerMock.Setup(provider => provider.GetService(typeof(ICamundaWorker)))
                .Returns(_workerMock.Object);
            _workerMock.Setup(w => w.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            using (var workerHostedService =
                new WorkerHostedService(_providerMock.Object, numberOfWorkers))
            {
                await workerHostedService.StartAsync(CancellationToken.None);
            }

            _providerMock.Verify(
                provider => provider.GetService(typeof(ICamundaWorker)),
                Times.Exactly(numberOfWorkers)
            );
            _workerMock.Verify(w => w.RunAsync(It.IsAny<CancellationToken>()), Times.Exactly(numberOfWorkers));
        }
    }
}
