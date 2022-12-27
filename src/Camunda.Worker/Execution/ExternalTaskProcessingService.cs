using System;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution;

internal sealed class ExternalTaskProcessingService : IExternalTaskProcessingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ExternalTaskDelegate _externalTaskDelegate;

    public ExternalTaskProcessingService(
        IServiceProvider serviceProvider,
        ExternalTaskDelegate externalTaskDelegate
    )
    {
        _serviceProvider = serviceProvider;
        _externalTaskDelegate = externalTaskDelegate;
    }

    public async Task ProcessAsync(
        ExternalTask externalTask,
        IExternalTaskClient externalTaskClient,
        CancellationToken cancellationToken
    )
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new ExternalTaskContext(
            externalTask,
            externalTaskClient,
            scope.ServiceProvider,
            cancellationToken
        );

        await _externalTaskDelegate(context);
    }
}
