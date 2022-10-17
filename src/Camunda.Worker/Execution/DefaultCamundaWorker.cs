using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution;

public sealed class DefaultCamundaWorker : ICamundaWorker
{
    private readonly IExternalTaskClient _externalTaskClient;
    private readonly IFetchAndLockRequestProvider _fetchAndLockRequestProvider;
    private readonly WorkerEvents _workerEvents;
    private readonly IServiceProvider _serviceProvider;
    private readonly WorkerHandlerDescriptor _workerHandlerDescriptor;
    private readonly ILogger<DefaultCamundaWorker> _logger;

    public DefaultCamundaWorker(
        IExternalTaskClient externalTaskClient,
        IFetchAndLockRequestProvider fetchAndLockRequestProvider,
        IOptions<WorkerEvents> workerEvents,
        IServiceProvider serviceProvider,
        WorkerHandlerDescriptor workerHandlerDescriptor,
        ILogger<DefaultCamundaWorker>? logger = null
    )
    {
        _externalTaskClient = Guard.NotNull(externalTaskClient, nameof(externalTaskClient));
        _fetchAndLockRequestProvider = Guard.NotNull(fetchAndLockRequestProvider, nameof(fetchAndLockRequestProvider));
        _workerEvents = Guard.NotNull(workerEvents, nameof(workerEvents)).Value;
        _serviceProvider = Guard.NotNull(serviceProvider, nameof(serviceProvider));
        _workerHandlerDescriptor = Guard.NotNull(workerHandlerDescriptor, nameof(workerHandlerDescriptor));
        _logger = logger ?? NullLogger<DefaultCamundaWorker>.Instance;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var externalTasks = await SelectAsync(cancellationToken);

            if (externalTasks is { Count: > 0 })
            {
                var tasks = new Task[externalTasks.Count];
                var i = 0;
                foreach (var externalTask in externalTasks)
                {
                    tasks[i++] = Task.Run(
                        () => ProcessExternalTaskAsync(externalTask, cancellationToken),
                        cancellationToken
                    );
                }

                await Task.WhenAll(tasks);
            }

            await _workerEvents.OnAfterProcessingAllTasks(_serviceProvider, cancellationToken);
        }
    }

    private async Task<List<ExternalTask>?> SelectAsync(CancellationToken cancellationToken)
    {
        await _workerEvents.OnBeforeFetchAndLock(_serviceProvider, cancellationToken);

        try
        {
            Log.Worker_Waiting(_logger);
            var fetchAndLockRequest = _fetchAndLockRequestProvider.GetRequest();
            var externalTasks = await _externalTaskClient.FetchAndLockAsync(fetchAndLockRequest, cancellationToken);
            Log.Worker_Locked(_logger, externalTasks.Count);
            return externalTasks;
        }
        catch (Exception e) when (!cancellationToken.IsCancellationRequested)
        {
            Log.Worker_FailedLocking(_logger, e.Message, e);
            await _workerEvents.OnFailedFetchAndLock(_serviceProvider, cancellationToken);
            return null;
        }
    }

    private async Task ProcessExternalTaskAsync(ExternalTask externalTask, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = new ExternalTaskContext(
            externalTask,
            _externalTaskClient,
            scope.ServiceProvider,
            cancellationToken
        );

        try
        {
            await _workerHandlerDescriptor.ExternalTaskDelegate(context);
        }
        catch (Exception e)
        {
            Log.Worker_FailedExecution(_logger, externalTask.Id, e);
        }
    }
}
