using System;
using System.Collections.Generic;
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
        private readonly CamundaWorkerOptions _options;
        private readonly SelectorOptions _selectorOptions;
        private readonly ILogger<ExternalTaskSelector> _logger;

        public ExternalTaskSelector(
            IExternalTaskClient client,
            ITopicsProvider topicsProvider,
            IOptions<CamundaWorkerOptions> options,
            IOptions<SelectorOptions> selectorOptions,
            ILogger<ExternalTaskSelector>? logger = null
        )
        {
            _client = Guard.NotNull(client, nameof(client));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _options = Guard.NotNull(options, nameof(options)).Value;
            _selectorOptions = Guard.NotNull(selectorOptions, nameof(selectorOptions)).Value;
            _logger = logger ?? NullLogger<ExternalTaskSelector>.Instance;
        }

        public async Task<IReadOnlyCollection<ExternalTask>> SelectAsync(
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                _logger.LogDebug("Waiting for external task");
                var fetchAndLockRequest = MakeRequestBody();
                var externalTasks = await PerformSelection(fetchAndLockRequest, cancellationToken);
                _logger.LogDebug("Locked {Count} external tasks", externalTasks.Count);
                return externalTasks;
            }
            catch (Exception e) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(e, "Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await DelayOnFailure(cancellationToken);
                return Array.Empty<ExternalTask>();
            }
        }

        private FetchAndLockRequest MakeRequestBody()
        {
            var topics = _topicsProvider.GetTopics();

            var fetchAndLockRequest = new FetchAndLockRequest(_options.WorkerId)
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
    }
}
