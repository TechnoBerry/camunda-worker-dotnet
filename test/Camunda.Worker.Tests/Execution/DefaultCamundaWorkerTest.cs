// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest
    {
        private readonly Mock<IExternalTaskCamundaClient> _apiClientMock = new Mock<IExternalTaskCamundaClient>();
        private readonly Mock<IGeneralExternalTaskHandler> _handlerMock = new Mock<IGeneralExternalTaskHandler>();
        private readonly Mock<ITopicsProvider> _topicsProviderMock = new Mock<ITopicsProvider>();

        private readonly IOptions<CamundaWorkerOptions> _options = Options.Create(new CamundaWorkerOptions
        {
            WorkerId = "testWorker",
            BaseUri = new Uri("http://test")
        });

        public DefaultCamundaWorkerTest()
        {
            _topicsProviderMock.Setup(provider => provider.GetTopics())
                .Returns(Enumerable.Empty<FetchAndLockRequest.Topic>());
        }

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
            _handlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestRunWithTask()
        {
            var cts = new CancellationTokenSource();

            ConfigureApiService(cts, new List<ExternalTask>
            {
                new ExternalTask
                {
                    Id = "1"
                }
            });

            var mockResult = new Mock<IExecutionResult>();

            mockResult
                .Setup(result => result.ExecuteResult(It.IsAny<ExternalTaskContext>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);

            _handlerMock
                .Setup(executor => executor.Process(It.IsAny<ExternalTask>()))
                .ReturnsAsync(mockResult.Object);

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _apiClientMock.Verify(
                client => client.FetchAndLock(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()),
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
                _handlerMock.Object,
                _options,
                _topicsProviderMock.Object,
                new NullLogger<DefaultCamundaWorker>()
            );
        }
    }
}
