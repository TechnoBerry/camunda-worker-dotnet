using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class ExternalTaskHandlerTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        private readonly Mock<FakeHandler> _handlerMock = new Mock<FakeHandler>
        {
            CallBase = true
        };

        [Fact]
        public async Task TestSuccessfulProcess()
        {
            var resultMock = new Mock<IExecutionResult>();
            _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>()))
                .ReturnsAsync(resultMock.Object);

            await _handlerMock.Object.HandleAsync(_contextMock.Object);

            resultMock.Verify(
                result => result.ExecuteResultAsync(It.IsAny<IExternalTaskContext>()),
                Times.Once()
            );
        }

        [Fact]
        public async Task TestFailedProcess()
        {
            _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>()))
                .ThrowsAsync(new Exception("An exception"));

            await _handlerMock.Object.HandleAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.ReportFailureAsync(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int?>(), It.IsAny<int?>()),
                Times.Once()
            );
        }

        public class FakeHandler : ExternalTaskHandler
        {
            public override Task<IExecutionResult> HandleAsync(ExternalTask externalTask)
            {
                throw new NotImplementedException();
            }
        }
    }
}
