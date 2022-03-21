using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution;

public class DefaultCamundaWorkerTest : IDisposable
{
    private readonly Mock<IHandler> _handlerMock = new();
    private readonly Mock<IExternalTaskClient> _clientMock = new();
    private readonly Mock<IFetchAndLockRequestProvider> _fetchAndLockRequestProviderMock = new();
    private readonly Mock<IWorkerEvents> _workerEventsMock = new();
    private readonly ServiceProvider _serviceProvider;
    private readonly DefaultCamundaWorker _worker;

    public DefaultCamundaWorkerTest()
    {
        _serviceProvider = new ServiceCollection().BuildServiceProvider();

        _fetchAndLockRequestProviderMock.Setup(provider => provider.GetRequest())
            .Returns(new FetchAndLockRequest("test"));

        _worker = new DefaultCamundaWorker(
            _clientMock.Object,
            _fetchAndLockRequestProviderMock.Object,
            Options.Create(new WorkerEvents
            {
                OnAfterProcessingAllTasks = _workerEventsMock.Object.OnAfterProcessingAllTasks
            }),
            _serviceProvider,
            new WorkerHandlerDescriptor(_handlerMock.Object.HandleAsync)
        );
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public async Task TestRun(int numberOfExternalTasks)
    {
        // Arrange
        var externalTasks = new Faker<ExternalTask>()
            .CustomInstantiator(faker => new ExternalTask(
                faker.Random.Guid().ToString(),
                faker.Random.Word(),
                faker.Random.Word())
            )
            .GenerateLazy(numberOfExternalTasks)
            .ToList();

        var cts = new CancellationTokenSource();

        _workerEventsMock
            .Setup(e => e.OnAfterProcessingAllTasks(It.IsAny<IServiceProvider>(), It.IsAny<CancellationToken>()))
            .Callback(cts.Cancel)
            .Returns(Task.CompletedTask);

        _clientMock
            .Setup(client => client.FetchAndLockAsync(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(externalTasks)
            .Verifiable();

        _handlerMock
            .Setup(executor => executor.HandleAsync(It.IsAny<IExternalTaskContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _worker.RunAsync(cts.Token);

        // Assert
        _handlerMock.Verify(
            executor => executor.HandleAsync(It.IsAny<IExternalTaskContext>()),
            Times.Exactly(numberOfExternalTasks)
        );
    }

    [Fact]
    public async Task TestCancelledSelection()
    {
        var cts = new CancellationTokenSource();
        var tcs = new TaskCompletionSource<List<ExternalTask>>();

        await using var reg = cts.Token.Register(() => tcs.SetCanceled());

        _clientMock
            .Setup(client =>
                client.FetchAndLockAsync(It.IsAny<FetchAndLockRequest>(), It.IsAny<CancellationToken>()))
            .Returns(tcs.Task);

        var resultTask = _worker.RunAsync(cts.Token);

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => resultTask);
        _clientMock.VerifyAll();
    }

    public interface IHandler
    {
        Task HandleAsync(IExternalTaskContext context);
    }

    public interface IWorkerEvents
    {
        public Task OnAfterProcessingAllTasks(IServiceProvider provider, CancellationToken ct);
    }
}
