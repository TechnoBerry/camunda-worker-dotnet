// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Api;
using Camunda.Worker.Core;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class DefaultCamundaWorkerTest
    {
        private readonly Mock<ICamundaApiClient> _apiClientMock = new Mock<ICamundaApiClient>();
        private readonly Mock<IExternalTaskExecutor> _executorMock = new Mock<IExternalTaskExecutor>();

        private readonly IOptions<CamundaWorkerOptions> _options = Options.Create(new CamundaWorkerOptions
        {
            WorkerId = "testWorker",
            BaseUri = new Uri("http://test")
        });

        [Fact]
        public async Task TestRunWithoutTasks()
        {
            var cts = new CancellationTokenSource();

            ConfigureApiService(cts, new List<ExternalTask>());

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _apiClientMock.Verify(
                client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _apiClientMock.VerifyNoOtherCalls();
            _executorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestRunWithSuccessfulExecution()
        {
            var cts = new CancellationTokenSource();

            ConfigureApiService(cts, new List<ExternalTask>
            {
                new ExternalTask
                {
                    Id = "1"
                }
            });

            _executorMock
                .Setup(executor => executor.Execute(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExecutionResult(new Dictionary<string, Variable>()));

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _apiClientMock.Verify(
                client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _apiClientMock.Verify(
                client => client.Complete("1", It.IsAny<CompleteRequest>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _apiClientMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestRunWithFailedExecution()
        {
            var cts = new CancellationTokenSource();

            ConfigureApiService(cts, new List<ExternalTask>
            {
                new ExternalTask
                {
                    Id = "1"
                }
            });

            _executorMock
                .Setup(executor => executor.Execute(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExecutionResult(new ArgumentNullException()));

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _apiClientMock.Verify(
                client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _apiClientMock.Verify(
                client => client.ReportFailure("1", It.IsAny<ReportFailureRequest>(), It.IsAny<CancellationToken>()),
                Times.Once()
            );
            _apiClientMock.VerifyNoOtherCalls();
        }

        private void ConfigureApiService(CancellationTokenSource cts, IList<ExternalTask> externalTasks)
        {
            _apiClientMock
                .Setup(client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    if (!externalTasks.Any())
                    {
                        cts.Cancel();
                    }
                })
                .ReturnsAsync(externalTasks);

            _apiClientMock
                .Setup(client => client.Complete(
                    It.IsAny<string>(),
                    It.IsAny<CompleteRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);

            _apiClientMock
                .Setup(client => client.ReportFailure(
                    It.IsAny<string>(),
                    It.IsAny<ReportFailureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);
        }

        private ICamundaWorker CreateWorker()
        {
            return new DefaultCamundaWorker(
                _apiClientMock.Object,
                _executorMock.Object,
                _options,
                Enumerable.Empty<HandlerDescriptor>(),
                new NullLogger<DefaultCamundaWorker>()
            );
        }
    }
}
