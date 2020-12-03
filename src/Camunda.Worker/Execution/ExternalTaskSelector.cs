using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CamundaWorkerOptions _options;
        private readonly ILogger<ExternalTaskSelector> _logger;

        public ExternalTaskSelector(
            IExternalTaskClient client,
            IOptions<CamundaWorkerOptions> options,
            ILogger<ExternalTaskSelector>? logger = null
        )
        {
            _client = Guard.NotNull(client, nameof(client));
            _options = Guard.NotNull(options, nameof(options)).Value;
            _logger = logger ?? NullLogger<ExternalTaskSelector>.Instance;
        }

        public async Task<IEnumerable<ExternalTask>> SelectAsync(
            IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                _logger.LogDebug("Waiting for external task");
                var fetchAndLockRequest = MakeRequestBody(topics);
                var externalTasks = await PerformSelection(fetchAndLockRequest, cancellationToken);
                _logger.LogDebug("Locked {Count} external tasks", externalTasks.Count);
                return externalTasks;
            }
            catch (Exception e) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(e,"Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await DelayOnFailure(cancellationToken);
                return Enumerable.Empty<ExternalTask>();
            }
        }

        private FetchAndLockRequest MakeRequestBody(IEnumerable<FetchAndLockRequest.Topic> topics)
        {
            var fetchAndLockRequest = new FetchAndLockRequest(_options.WorkerId)
            {
                UsePriority = true,
                AsyncResponseTimeout = _options.AsyncResponseTimeout,
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
