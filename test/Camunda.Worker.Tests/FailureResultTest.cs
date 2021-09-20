using System;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class FailureResultTest
    {
        private readonly ExternalTask _externalTask;
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<IExternalTaskContext> _contextMock = new();

        public FailureResultTest()
        {
            _externalTask = new Faker<ExternalTask>()
                .CustomInstantiator(faker => new ExternalTask(
                    faker.Random.Guid().ToString(),
                    faker.Random.Word(),
                    faker.Random.Word())
                )
                .Generate();
            _contextMock.Setup(ctx => ctx.Task).Returns(_externalTask);
            _contextMock.Setup(ctx => ctx.Client).Returns(_clientMock.Object);
        }

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            // Arrange
            _clientMock
                .Setup(client => client.ReportFailureAsync(
                    _externalTask.Id, It.IsAny<ReportFailureRequest>(), default
                ))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = new FailureResult(new Exception("Message"));

            // Act
            await result.ExecuteResultAsync(_contextMock.Object);

            // Assert
            _clientMock.Verify();
            _clientMock.VerifyNoOtherCalls();
        }
    }
}
