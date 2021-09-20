using System;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class HandlerInvokerTest
    {
        private readonly Mock<IExternalTaskHandler> _handlerMock = new();
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<IExternalTaskContext> _contextMock = new();
        private readonly HandlerInvoker _handlerInvoker;

        public HandlerInvokerTest()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var externalTask = new Faker<ExternalTask>()
                .CustomInstantiator(faker => new ExternalTask(
                    faker.Random.Guid().ToString(),
                    faker.Random.Word(),
                    faker.Random.Word())
                )
                .Generate();

            _contextMock.SetupGet(ctx => ctx.ServiceProvider)
                .Returns(serviceProvider);
            _contextMock.SetupGet(ctx => ctx.Task)
                .Returns(externalTask);
            _contextMock.SetupGet(ctx => ctx.Client)
                .Returns(_clientMock.Object);
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
            _clientMock.Verify(
                client => client.ReportFailureAsync(It.IsAny<string>(), It.IsAny<ReportFailureRequest>(), default),
                Times.Once()
            );
        }
    }
}
