using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskSelector : IExternalTaskSelector
    {
        private readonly IExternalTaskClient _client;
        private readonly ITopicsProvider _topicsProvider;
        private readonly CamundaWorkerOptions _workerOptions;
        private readonly SelectorOptions _selectorOptions;
        private readonly ILogger<ExternalTaskSelector> _logger;

        public ExternalTaskSelector(
            IExternalTaskClient client,
            ITopicsProvider topicsProvider,
            CamundaWorkerOptions workerOptions,
            IOptions<SelectorOptions> selectorOptions,
            ILogger<ExternalTaskSelector>? logger = null
        )
        {
            _client = Guard.NotNull(client, nameof(client));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _workerOptions = Guard.NotNull(workerOptions, nameof(workerOptions));
            _selectorOptions = Guard.NotNull(selectorOptions, nameof(selectorOptions)).Value;
            _logger = logger ?? NullLogger<ExternalTaskSelector>.Instance;
        }

        public async Task<IReadOnlyCollection<ExternalTask>> SelectAsync(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                Log.Waiting(_logger);
                var fetchAndLockRequest = MakeRequestBody();
                var externalTasks = await PerformSelection(fetchAndLockRequest, cancellationToken);
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

            var fetchAndLockRequest = new FetchAndLockRequest(_workerOptions.WorkerId, _selectorOptions.MaxTasks)
            {
                UsePriority = _selectorOptions.UsePriority,
                AsyncResponseTimeout = _selectorOptions.AsyncResponseTimeout,
                Topics = topics
            };

            return fetchAndLockRequest;
        }

        private async Task<List<ExternalTask>> PerformSelection(
            FetchAndLockRequest request,
            CancellationToken cancellationToken
        )
        {
            var externalTasks = await _client.FetchAndLockAsync(request, cancellationToken);
            return externalTasks;
        }

        private static Task DelayOnFailure(CancellationToken cancellationToken) =>
            Task.Delay(10_000, cancellationToken);

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

            public static void Waiting(ILogger logger) => _waiting(logger, null);

            public static void Locked(ILogger logger, int count) => _locked(logger, count, null);

            public static void FailedLocking(ILogger logger, Exception e) => _failedLocking(logger, e.Message, e);
        }
    }
}
