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

        private readonly Mock<IExceptionHandler> _exceptionHandlerMock = new Mock<IExceptionHandler>();

        public GeneralExternalTaskHandlerTest()
        {
            _scopeFactoryMock.Setup(factory => factory.CreateScope()).Returns(_scopeMock.Object);
            _scopeMock.SetupGet(scope => scope.ServiceProvider).Returns(_providerMock.Object);
        }

        [Fact]
        public async Task TestExecute()
        {
            var handlerMock = MakeHandlerMock();

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ReturnsAsync(new CompleteResult(new Dictionary<string, Variable>
                {
                    ["DONE"] = new Variable(true)
                }));

            var executor = MakeExecutor();

            var result = await executor.Process(new ExternalTask("1", "testWorker", "testTopic"));

            handlerMock.VerifyAll();
            Assert.IsType<CompleteResult>(result);
        }

        [Fact]
        public async Task TestExecuteWithTransformedException()
        {
            var handlerMock = MakeHandlerMock();

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ThrowsAsync(new Exception("Test exception"));
            IExecutionResult failureResult = new FailureResult("TEST", "TEST");
            _exceptionHandlerMock
                .Setup(handler => handler.TryTransformToResult(It.IsAny<Exception>(), out failureResult))
                .Returns(true);

            var executor = MakeExecutor();

            var result = await executor.Process(new ExternalTask("1", "testWorker", "testTopic"));

            handlerMock.VerifyAll();
            _exceptionHandlerMock.VerifyAll();
            Assert.IsType<FailureResult>(result);
        }

        [Fact]
        public async Task TestExecuteWithUntransformedException()
        {
            var handlerMock = MakeHandlerMock();

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ThrowsAsync(new Exception("Test exception"));
            IExecutionResult failureResult = new FailureResult("TEST", "TEST");
            _exceptionHandlerMock
                .Setup(handler => handler.TryTransformToResult(It.IsAny<Exception>(), out failureResult))
                .Returns(false);

            var executor = MakeExecutor();

            await Assert.ThrowsAsync<Exception>(async () =>
                await executor.Process(new ExternalTask("1", "testWorker", "testTopic")));

            handlerMock.VerifyAll();
            _exceptionHandlerMock.VerifyAll();
        }

        private Mock<IExternalTaskHandler> MakeHandlerMock()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();
            _handlerFactoryProviderMock.Setup(factory => factory.GetHandlerFactory(It.IsAny<ExternalTask>()))
                .Returns(provider => handlerMock.Object);
            return handlerMock;
        }

        [Fact]
        public async Task TestExecuteWithNullArg()
        {
            var executor = MakeExecutor();

            await Assert.ThrowsAsync<ArgumentNullException>(() => executor.Process(null));
        }

        private IGeneralExternalTaskHandler MakeExecutor()
        {
            return new GeneralExternalTaskHandler(
                _scopeFactoryMock.Object,
                _handlerFactoryProviderMock.Object,
                _exceptionHandlerMock.Object
            );
        }
    }
}
