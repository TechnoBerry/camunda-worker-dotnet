using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

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
            _serviceProviderMock.Object,
            default
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
    }
}
