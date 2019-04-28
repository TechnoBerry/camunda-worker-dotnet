using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class FailureResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            Expression<Func<IExternalTaskContext, Task>> expression = context => context.ReportFailureAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            );

            _contextMock
                .Setup(expression)
                .Returns(Task.CompletedTask);

            var result = new FailureResult(new Exception("Message"));

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(expression, Times.Once());
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
