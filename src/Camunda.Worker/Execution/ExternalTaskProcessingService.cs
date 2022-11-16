using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution;

internal sealed class ExternalTaskProcessingService : IExternalTaskProcessingService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IExternalTaskClient _externalTaskClient;
    private readonly WorkerHandlerDescriptor _workerHandlerDescriptor;

    public ExternalTaskProcessingService(
        IServiceScopeFactory scopeFactory,
        IExternalTaskClient externalTaskClient,
        WorkerHandlerDescriptor workerHandlerDescriptor
    )
    {
        _scopeFactory = scopeFactory;
        _externalTaskClient = externalTaskClient;
        _workerHandlerDescriptor = workerHandlerDescriptor;
    }

    public async Task ProcessAsync(ExternalTask externalTask, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var context = new ExternalTaskContext(
            externalTask,
            _externalTaskClient,
            scope.ServiceProvider,
            cancellationToken
        );

        await _workerHandlerDescriptor.ExternalTaskDelegate(context);
    }
}
