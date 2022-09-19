using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Camunda.Worker.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class HandlerInvokerTest : IDisposable
{
    private readonly Mock<IExternalTaskHandler> _handlerMock = new();
    private readonly Mock<IExternalTaskClient> _clientMock = new();
    private readonly Mock<IExternalTaskContext> _contextMock = new();
    private readonly CancellationTokenSource _processingAborted = new();
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
        _contextMock.SetupGet(ctx => ctx.ProcessingAborted)
            .Returns(_processingAborted.Token);
        _handlerInvoker = new HandlerInvoker(_handlerMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task ShouldExecuteResultReturnedFromHandler()
    {
        // Arrange
        var resultMock = new Mock<IExecutionResult>();

        _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultMock.Object);

        // Act
        await _handlerInvoker.InvokeAsync();

        // Assert
        resultMock.Verify(result => result.ExecuteResultAsync(It.IsAny<IExternalTaskContext>()), Times.Once());
    }

    [Fact]
    public async Task ShouldReportFailureIfHandlerFails()
    {
        // Arrange
        _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        await _handlerInvoker.InvokeAsync();

        // Assert
        _clientMock.Verify(
            client => client.ReportFailureAsync(It.IsAny<string>(), It.IsAny<ReportFailureRequest>(), default),
            Times.Once()
        );
    }

    [Fact]
    public async Task ShouldNotReportFailureIfHandlerCancelled()
    {
        // Arrange
        var taskCompletionSource = new TaskCompletionSource<IExecutionResult>();
        await using var ctReg = _processingAborted.Token.Register(() =>
        {
            taskCompletionSource.TrySetCanceled(_processingAborted.Token);
        });

        _handlerMock.Setup(handler => handler.HandleAsync(It.IsAny<ExternalTask>(), It.IsAny<CancellationToken>()))
            .Returns(taskCompletionSource.Task);

        // Act
        var invokerTask = _handlerInvoker.InvokeAsync();
        await Task.Delay(100);
        _processingAborted.Cancel();

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => invokerTask);
    }

    public void Dispose()
    {
        _processingAborted.Dispose();
    }
}
