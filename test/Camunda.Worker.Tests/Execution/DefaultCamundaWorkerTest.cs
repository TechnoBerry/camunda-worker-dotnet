#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest
    {
        private readonly Mock<IExternalTaskCamundaClient> _apiClientMock = new Mock<IExternalTaskCamundaClient>();
        private readonly Mock<IExternalTaskRouter> _routerMock = new Mock<IExternalTaskRouter>();
        private readonly Mock<ITopicsProvider> _topicsProviderMock = new Mock<ITopicsProvider>();
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new Mock<IServiceScopeFactory>();

        private readonly IOptions<CamundaWorkerOptions> _options = Options.Create(new CamundaWorkerOptions
        {
            WorkerId = "testWorker",
            BaseUri = new Uri("http://test"),
            AsyncResponseTimeout = 5_000
        });

        public DefaultCamundaWorkerTest()
        {
            _topicsProviderMock.Setup(provider => provider.GetTopics())
                .Returns(Enumerable.Empty<FetchAndLockRequest.Topic>());

            _scopeFactoryMock.Setup(factory => factory.CreateScope())
                .Returns(() =>
                {
                    var scopeMock = new Mock<IServiceScope>();
                    return scopeMock.Object;
                });
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
            _routerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestRunWithTask()
        {
            var cts = new CancellationTokenSource();

            ConfigureApiService(cts, new List<ExternalTask>
            {
                new ExternalTask("test", "test", "test")
            });

            _routerMock
                .Setup(executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _scopeFactoryMock.Verify(factory => factory.CreateScope(), Times.Once());
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
        }

        private ICamundaWorker CreateWorker()
        {
            return new DefaultCamundaWorker(
                _apiClientMock.Object,
                _routerMock.Object,
                _topicsProviderMock.Object,
                _scopeFactoryMock.Object,
                _options
            );
        }
    }
}
