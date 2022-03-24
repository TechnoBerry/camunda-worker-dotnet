using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            await _workerEvents.OnBeforeFetchAndLock(_serviceProvider, cancellationToken);

            var externalTasks = await SelectAsync(cancellationToken);

            if (externalTasks != null && externalTasks.Count != 0)
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
        try
        {
            LogHelper.LogWaiting(_logger);
            var fetchAndLockRequest = _fetchAndLockRequestProvider.GetRequest();
            var externalTasks = await _externalTaskClient.FetchAndLockAsync(fetchAndLockRequest, cancellationToken);
            LogHelper.LogLocked(_logger, externalTasks.Count);
            return externalTasks;
        }
        catch (Exception e) when (!cancellationToken.IsCancellationRequested)
        {
            LogHelper.LogFailedLocking(_logger, e);
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
            LogHelper.LogFailedExecution(_logger, externalTask.Id, e);
        }
    }

    [ExcludeFromCodeCoverage]
    private static class LogHelper
    {
        private static readonly Action<ILogger, Exception?> Waiting =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(0),
                "Waiting for external task"
            );

        private static readonly Action<ILogger, int, Exception?> Locked =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(0),
                "Locked {Count} external tasks"
            );

        private static readonly Action<ILogger, string, Exception?> FailedLocking =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(0),
                "Failed locking of external tasks. Reason: \"{Reason}\""
            );

        private static readonly Action<ILogger, string, Exception?> FailedExecution =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(0),
                "Failed execution of task {Id}"
            );

        public static void LogWaiting(ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                Waiting(logger, null);
            }
        }

        public static void LogLocked(ILogger logger, int count)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                Locked(logger, count, null);
            }
        }

        public static void LogFailedLocking(ILogger logger, Exception e)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                FailedLocking(logger, e.Message, e);
            }
        }

        public static void LogFailedExecution(ILogger logger, string externalTaskId, Exception e)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                FailedExecution(logger, externalTaskId, e);
            }
        }
    }
}
