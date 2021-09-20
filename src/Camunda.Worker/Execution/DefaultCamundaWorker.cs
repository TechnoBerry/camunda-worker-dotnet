using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskClient _externalTaskClient;
        private readonly ITopicsProvider _topicsProvider;
        private readonly CamundaWorkerOptions _workerOptions;
        private readonly FetchAndLockOptions _fetchAndLockOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly WorkerHandlerDescriptor _workerHandlerDescriptor;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(
            IExternalTaskClient externalTaskClient,
            ITopicsProvider topicsProvider,
            CamundaWorkerOptions workerOptions,
            IOptions<FetchAndLockOptions> fetchAndLockOptions,
            IServiceScopeFactory serviceScopeFactory,
            WorkerHandlerDescriptor workerHandlerDescriptor,
            ILogger<DefaultCamundaWorker>? logger = null
        )
        {
            _externalTaskClient = Guard.NotNull(externalTaskClient, nameof(externalTaskClient));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _workerOptions = Guard.NotNull(workerOptions, nameof(workerOptions));
            _fetchAndLockOptions = Guard.NotNull(fetchAndLockOptions, nameof(fetchAndLockOptions)).Value;
            _serviceScopeFactory = Guard.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
            _workerHandlerDescriptor = Guard.NotNull(workerHandlerDescriptor, nameof(workerHandlerDescriptor));
            _logger = logger ?? NullLogger<DefaultCamundaWorker>.Instance;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTasks = await SelectAsync(cancellationToken);

                var executableTasks = externalTasks
                    .Select(ProcessExternalTask)
                    .ToList();

                await Task.WhenAll(executableTasks);
            }
        }

        private async Task<IReadOnlyCollection<ExternalTask>> SelectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Log.Waiting(_logger);
                var fetchAndLockRequest = MakeRequestBody();
                var externalTasks = await _externalTaskClient.FetchAndLockAsync(fetchAndLockRequest, cancellationToken);
                Log.Locked(_logger, externalTasks.Count);
                return externalTasks;
            }
            catch (Exception e) when (!cancellationToken.IsCancellationRequested)
            {
                Log.FailedLocking(_logger, e);
                await DelayOnFailure(cancellationToken);
                return Array.Empty<ExternalTask>();
            }
        }

        private FetchAndLockRequest MakeRequestBody()
        {
            var topics = _topicsProvider.GetTopics();

            var fetchAndLockRequest = new FetchAndLockRequest(_workerOptions.WorkerId, _fetchAndLockOptions.MaxTasks)
            {
                UsePriority = _fetchAndLockOptions.UsePriority,
                AsyncResponseTimeout = _fetchAndLockOptions.AsyncResponseTimeout,
                Topics = topics
            };

            return fetchAndLockRequest;
        }

        private static Task DelayOnFailure(CancellationToken cancellationToken) =>
            Task.Delay(10_000, cancellationToken);

        private async Task ProcessExternalTask(ExternalTask externalTask)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = new ExternalTaskContext(externalTask, _externalTaskClient, scope.ServiceProvider);

            try
            {
                await _workerHandlerDescriptor.ExternalTaskDelegate(context);
            }
            catch (Exception e)
            {
                Log.FailedExecution(_logger, externalTask.Id, e);
            }
        }

        [ExcludeFromCodeCoverage]
        private static class Log
        {
            private static readonly Action<ILogger, Exception?> _waiting =
                LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(0),
                    "Waiting for external task"
                );

            private static readonly Action<ILogger, int, Exception?> _locked =
                LoggerMessage.Define<int>(
                    LogLevel.Debug,
                    new EventId(0),
                    "Locked {Count} external tasks"
                );

            private static readonly Action<ILogger, string, Exception?> _failedLocking =
                LoggerMessage.Define<string>(
                    LogLevel.Warning,
                    new EventId(0),
                    "Failed locking of external tasks. Reason: \"{Reason}\""
                );

            private static readonly Action<ILogger, string, Exception?> _failedExecution =
                LoggerMessage.Define<string>(
                    LogLevel.Warning,
                    new EventId(0),
                    "Failed execution of task {Id}"
                );

            public static void Waiting(ILogger logger) => _waiting(logger, null);

            public static void Locked(ILogger logger, int count) => _locked(logger, count, null);

            public static void FailedLocking(ILogger logger, Exception e) => _failedLocking(logger, e.Message, e);

            public static void FailedExecution(ILogger logger, string externalTaskId, Exception e) =>
                _failedExecution(logger, externalTaskId, e);
        }
    }
}
