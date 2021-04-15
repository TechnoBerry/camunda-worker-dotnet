using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class HandlerInvokerTest
    {
        private readonly Mock<IExternalTaskHandler> _handlerMock = new();
        private readonly Mock<IExternalTaskContext> _contextMock = new();
        private readonly HandlerInvoker _handlerInvoker;

        public HandlerInvokerTest()
        {
            _handlerInvoker = new HandlerInvoker(_handlerMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task TestExecuteReturnedFromHandlerResult()
        {
            // Arrange
            var resultMock = new Mock<IExecutionResult>();

            _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>(), default))
                .ReturnsAsync(resultMock.Object);

            // Act
            await _handlerInvoker.InvokeAsync();

            // Assert
            resultMock.Verify(result => result.ExecuteResultAsync(It.IsAny<IExternalTaskContext>()), Times.Once());
        }

        [Fact]
        public async Task TestReportFailureIfHandlerFails()
        {
            // Arrange
            _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>(), default))
                .ThrowsAsync(new Exception());

            // Act
            await _handlerInvoker.InvokeAsync();

            // Assert
            _contextMock.Verify(
                context => context.ReportFailureAsync(It.IsAny<string>(), It.IsAny<string>(), null, null),
                Times.Once()
            );
        }
    }
}
