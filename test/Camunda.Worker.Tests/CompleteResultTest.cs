using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CompleteResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new();

        public CompleteResultTest()
        {
            _contextMock.SetupGet(c => c.Task).Returns(new ExternalTask("", "", ""));
            _contextMock.SetupGet(c => c.ServiceProvider).Returns(new ServiceCollection().BuildServiceProvider());
        }

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            // Arrange
            _contextMock
                .Setup(context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = new CompleteResult
            {
                Variables = new Dictionary<string, Variable>()
            };

            //Act
            await result.ExecuteResultAsync(_contextMock.Object);

            // Assert
            _contextMock.Verify();
            _contextMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestExecuteResultWithFailedCompletion()
        {
            _contextMock
                .Setup(context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ))
                .ThrowsAsync(new ClientException(new ErrorResponse
                {
                    Type = "an error type",
                    Message = "an error message"
                }, HttpStatusCode.InternalServerError))
                .Verifiable();

            _contextMock
                .Setup(context => context.ReportFailureAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = new CompleteResult
            {
                Variables = new Dictionary<string, Variable>()
            };

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.Verify();
            _contextMock.VerifyGet(c => c.ServiceProvider, Times.Once());
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
