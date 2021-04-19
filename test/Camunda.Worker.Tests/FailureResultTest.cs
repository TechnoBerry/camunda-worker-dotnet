using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class FailureResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            // Arrange
            _contextMock
                .Setup(context => context.ReportFailureAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = new FailureResult(new Exception("Message"));

            // Act
            await result.ExecuteResultAsync(_contextMock.Object);

            // Assert
            _contextMock.Verify();
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
