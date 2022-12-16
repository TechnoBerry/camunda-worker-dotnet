using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution;

internal sealed class ExternalTaskProcessingService : IExternalTaskProcessingService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerHandlerDescriptor _workerHandlerDescriptor;

    public ExternalTaskProcessingService(
        IServiceScopeFactory scopeFactory,
        WorkerHandlerDescriptor workerHandlerDescriptor
    )
    {
        _scopeFactory = scopeFactory;
        _workerHandlerDescriptor = workerHandlerDescriptor;
    }

    public async Task ProcessAsync(
        ExternalTask externalTask,
        IExternalTaskClient externalTaskClient,
        CancellationToken cancellationToken
    )
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var context = new ExternalTaskContext(
            externalTask,
            externalTaskClient,
            scope.ServiceProvider,
            cancellationToken
        );

        await _workerHandlerDescriptor.ExternalTaskDelegate(context);
    }
}
