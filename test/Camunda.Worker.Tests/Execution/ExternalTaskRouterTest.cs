#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskRouterTest
    {
        private readonly Mock<IServiceProvider> _providerMock = new Mock<IServiceProvider>();
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        private readonly Mock<IHandlerFactoryProvider>
            _handlerFactoryProviderMock = new Mock<IHandlerFactoryProvider>();

        private readonly Mock<IExceptionHandler> _exceptionHandlerMock = new Mock<IExceptionHandler>();

        public ExternalTaskRouterTest()
        {
            _contextMock.SetupGet(context => context.ServiceProvider).Returns(_providerMock.Object);
            _contextMock.SetupGet(context => context.Task).Returns(new ExternalTask("1", "testWorker", "testTopic"));
        }

        [Fact]
        public async Task TestExecute()
        {
            var handlerMock = MakeHandlerMock();

            var resultMock = new Mock<IExecutionResult>();

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ReturnsAsync(resultMock.Object);

            var executor = MakeExecutor();

            await executor.Execute(_contextMock.Object);

            resultMock.Verify(result => result.ExecuteResultAsync(It.IsAny<IExternalTaskContext>()), Times.Once());
        }

        [Fact]
        public async Task TestExecuteWithTransformedException()
        {
            var handlerMock = MakeHandlerMock();

            handlerMock.Setup(handler => handler.Process(It.IsAny<ExternalTask>()))
                .ThrowsAsync(new Exception("Test exception"));

            var transformedResultMock = new Mock<IExecutionResult>();
            var transformedResult = transformedResultMock.Object;
            _exceptionHandlerMock
                .Setup(handler => handler.TryTransformToResult(It.IsAny<Exception>(), out transformedResult))
                .Returns(true);

            var executor = MakeExecutor();

            await executor.Execute(_contextMock.Object);

            handlerMock.VerifyAll();
            _exceptionHandlerMock.VerifyAll();
            transformedResultMock.Verify(
                result => result.ExecuteResultAsync(It.IsAny<IExternalTaskContext>()),
                Times.Once()
            );
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

            await Assert.ThrowsAsync<Exception>(async () => await executor.Execute(_contextMock.Object));

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

            await Assert.ThrowsAsync<ArgumentNullException>(() => executor.Execute(null));
        }

        private IExternalTaskRouter MakeExecutor()
        {
            return new ExternalTaskRouter(
                _handlerFactoryProviderMock.Object,
                _exceptionHandlerMock.Object
            );
        }
    }
}
