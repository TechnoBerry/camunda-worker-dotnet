#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class GeneralExternalTaskHandlerTest
    {
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        private readonly Mock<IServiceScope> _scopeMock = new Mock<IServiceScope>();
        private readonly Mock<IServiceProvider> _providerMock = new Mock<IServiceProvider>();

        private readonly Mock<IHandlerFactoryProvider>
            _handlerFactoryProviderMock = new Mock<IHandlerFactoryProvider>();

        public GeneralExternalTaskHandlerTest()
        {
            _scopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(_scopeMock.Object);
            _scopeMock.SetupGet(scope => scope.ServiceProvider).Returns(_providerMock.Object);
        }

        [Fact]
        public async Task TestExecute()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();
            _handlerFactoryProviderMock.Setup(factory => factory.GetHandlerFactory(It.IsAny<ExternalTask>()))
                .Returns(provider => handlerMock.Object);

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ReturnsAsync(new CompleteResult(new Dictionary<string, Variable>
                {
                    ["DONE"] = new Variable(true)
                }));

            var executor = new GeneralExternalTaskHandler(
                _scopeFactoryMock.Object,
                _handlerFactoryProviderMock.Object
            );

            await executor.Process(new ExternalTask("1", "testWorker", "testTopic")
            {
                Variables = new Dictionary<string, Variable>()
            });

            handlerMock.Verify(
                handler => handler.Process(It.IsAny<ExternalTask>()),
                Times.Once()
            );
        }

        [Fact]
        public async Task TestExecuteWithNullArg()
        {
            var executor = new GeneralExternalTaskHandler(
                _scopeFactoryMock.Object,
                _handlerFactoryProviderMock.Object
            );

            await Assert.ThrowsAsync<ArgumentNullException>(() => executor.Process(null));
        }
    }
}
