using System;
using System.Collections.Generic;
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
    public sealed class ExternalTaskSelector : IExternalTaskSelector
    {
        private readonly IServiceProvider _provider;
        private readonly CamundaWorkerOptions _options;
        private readonly ILogger<ExternalTaskSelector> _logger;

        public ExternalTaskSelector(
            IServiceProvider provider,
            IOptions<CamundaWorkerOptions> options,
            ILogger<ExternalTaskSelector>? logger = null
        )
        {
            _provider = Guard.NotNull(provider, nameof(provider));
            _options = Guard.NotNull(options, nameof(options)).Value;
            _logger = logger ?? NullLogger<ExternalTaskSelector>.Instance;
        }

        public async Task<IEnumerable<ExternalTask>> SelectAsync(
            IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default
        )
        {
            var client = _provider.GetRequiredService<IExternalTaskClient>();

            try
            {
                _logger.LogDebug("Waiting for external task");
                var fetchAndLockRequest = MakeRequestBody(topics);
                var externalTasks = await PerformSelection(client, fetchAndLockRequest, cancellationToken);
                var externalTaskList = externalTasks.ToList();
                _logger.LogDebug("Locked {Count} external tasks", externalTaskList.Count);
                return externalTaskList;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                _logger.LogWarning(e,"Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await DelayOnFailure(cancellationToken);
                return Enumerable.Empty<ExternalTask>();
            }
            finally
            {
                client?.Dispose();
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

        private async Task<IEnumerable<ExternalTask>> PerformSelection(
            IExternalTaskClient client,
            FetchAndLockRequest request,
            CancellationToken cancellationToken
        )
        {
            var externalTasks = await client.FetchAndLockAsync(request, cancellationToken);
            return externalTasks;
        }

        private static Task DelayOnFailure(CancellationToken cancellationToken) =>
            Task.Delay(10_000, cancellationToken);
    }
}
