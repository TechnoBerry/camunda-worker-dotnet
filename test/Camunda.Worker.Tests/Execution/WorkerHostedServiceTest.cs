// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class WorkerHostedServiceTest
    {
        private readonly Mock<ICamundaWorker> _workerMock = new Mock<ICamundaWorker>();

        [Fact]
        public async Task TestRunWorkerOnStart()
        {
            _workerMock.Setup(w => w.Run(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            using (var workerHostedService = new WorkerHostedService(_workerMock.Object))
            {
                await workerHostedService.StartAsync(CancellationToken.None);
            }

            _workerMock.Verify(w => w.Run(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
