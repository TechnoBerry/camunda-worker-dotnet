using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskContextTest
    {
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly ExternalTaskContext _context;

        public ExternalTaskContextTest()
        {
            _context = new ExternalTaskContext(
                new ExternalTask("testId", "testWorker", "testTopic"),
                _clientMock.Object,
                _serviceProviderMock.Object
            );
        }

        [Fact]
        public async Task TestExtendLockAsync()
        {
            // Arrange
            _clientMock.Setup(client =>
                client.ExtendLockAsync(It.IsAny<string>(), It.IsNotNull<ExtendLockRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _context.ExtendLockAsync(5_000);

            // Assert
            _clientMock.VerifyAll();
            Assert.False(_context.Completed);
        }

        [Fact]
        public async Task TestCompleteAsync()
        {
            // Arrange
            _clientMock.Setup(client =>
                client.CompleteAsync(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _context.CompleteAsync(new Dictionary<string, Variable>());

            // Assert
            _clientMock.VerifyAll();
            Assert.True(_context.Completed);
        }

        [Fact]
        public async Task TestReportFailureAsync()
        {
            // Arrange
            _clientMock.Setup(client =>
                client.ReportFailureAsync(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(),
                    CancellationToken.None)
            ).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _context.ReportFailureAsync("message", "details");

            // Assert
            _clientMock.VerifyAll();
            Assert.True(_context.Completed);
        }

        [Fact]
        public async Task TestReportBpmnErrorAsync()
        {
            // Arrange
            _clientMock.Setup(client =>
                client.ReportBpmnErrorAsync(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(),
                    CancellationToken.None)
            ).Returns(Task.CompletedTask).Verifiable();

            // Act
            await _context.ReportBpmnErrorAsync("code", "message");

            // Assert
            _clientMock.VerifyAll();
            Assert.True(_context.Completed);
        }

        [Theory]
        [MemberData(nameof(GetDoubleCompletionArguments))]
        public async Task TestDoubleCompletion(
            Func<IExternalTaskContext, Task> first,
            Func<IExternalTaskContext, Task> second
        )
        {
            // Arrange
            _clientMock.Setup(client =>
                client.CompleteAsync(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportFailureAsync(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(),
                    CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportBpmnErrorAsync(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(),
                    CancellationToken.None)
            ).Returns(Task.CompletedTask);

            await first(_context);

            // Act & Assert
            await Assert.ThrowsAsync<CamundaWorkerException>(() => second(_context));
        }

        public static IEnumerable<object[]> GetDoubleCompletionArguments()
        {
            return GetCompletionFunctions()
                .Join(GetCompletionFunctions(), _ => true, _ => true, (a, b) => new object[] {a, b});
        }

        private static IEnumerable<Func<IExternalTaskContext, Task>> GetCompletionFunctions()
        {
            yield return ctx => ctx.CompleteAsync(new Dictionary<string, Variable>());
            yield return ctx => ctx.ReportFailureAsync("message", "details");
            yield return ctx => ctx.ReportBpmnErrorAsync("core", "message");
        }
    }
}
