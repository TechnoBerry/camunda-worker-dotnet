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
        private readonly Mock<IExternalTaskSelector> _selectorMock = new Mock<IExternalTaskSelector>();

        public DefaultCamundaWorkerTest()
        {
            _topicsProviderMock.Setup(provider => provider.GetTopics())
                .Returns(Enumerable.Empty<FetchAndLockRequest.Topic>());

            var providerMock = new Mock<IServiceProvider>();
            providerMock.Setup(provider => provider.GetService(typeof(IExternalTaskCamundaClient)))
                .Returns(_apiClientMock.Object);

            _scopeFactoryMock.Setup(factory => factory.CreateScope())
                .Returns(() =>
                {
                    var scopeMock = new Mock<IServiceScope>();
                    scopeMock.SetupGet(scope => scope.ServiceProvider).Returns(providerMock.Object);
                    return scopeMock.Object;
                });
        }

        [Fact]
        public async Task TestRunWithoutTasks()
        {
            var cts = new CancellationTokenSource();

            ConfigureSelector(cts, new List<ExternalTask>());

            var worker = CreateWorker();

            await worker.Run(cts.Token);

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

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _scopeFactoryMock.Verify(factory => factory.CreateScope(), Times.Once());
            _selectorMock.VerifyAll();
        }

        private void ConfigureSelector(CancellationTokenSource cts, IList<ExternalTask> externalTasks)
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

            _selectorMock
                .Setup(selector => selector.SelectAsync(
                    It.IsAny<IEnumerable<FetchAndLockRequest.Topic>>(),
                    It.IsAny<CancellationToken>()
                ))
                .Callback(cts.Cancel)
                .ReturnsAsync(externalTasks);
        }

        private ICamundaWorker CreateWorker()
        {
            return new DefaultCamundaWorker(
                _routerMock.Object,
                _topicsProviderMock.Object,
                _selectorMock.Object,
                _scopeFactoryMock.Object
            );
        }
    }
}
