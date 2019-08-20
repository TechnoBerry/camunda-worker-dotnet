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
    public class ExternalTaskSelector : IExternalTaskSelector
    {
        public ExternalTaskSelector(IExternalTaskClient client, IOptions<CamundaWorkerOptions> options,
            ILogger<ExternalTaskSelector> logger = null)
        {
            Client = Guard.NotNull(client, nameof(client));
            Options = Guard.NotNull(options, nameof(options)).Value;
            Logger = logger ?? NullLogger<ExternalTaskSelector>.Instance;
        }

        protected IExternalTaskClient Client { get; }
        protected CamundaWorkerOptions Options { get; }
        protected ILogger<ExternalTaskSelector> Logger { get; }

        public async Task<IEnumerable<ExternalTask>> SelectAsync(
            IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                Logger.LogDebug("Waiting for external task");

                var fetchAndLockRequest = MakeRequestBody(topics);
                var externalTasks = await PerformSelection(fetchAndLockRequest, cancellationToken);
                var externalTaskList = externalTasks.ToList();

                Logger.LogDebug("Locked {Count} external tasks", externalTaskList.Count);

                return externalTaskList;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Logger.LogWarning("Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await DelayOnFailure(cancellationToken);
                return Enumerable.Empty<ExternalTask>();
            }
        }

        protected virtual FetchAndLockRequest MakeRequestBody(IEnumerable<FetchAndLockRequest.Topic> topics)
        {
            var fetchAndLockRequest = new FetchAndLockRequest(Options.WorkerId)
            {
                UsePriority = true,
                AsyncResponseTimeout = Options.AsyncResponseTimeout,
                Topics = topics
            };

            return fetchAndLockRequest;
        }

        protected virtual async Task<IEnumerable<ExternalTask>> PerformSelection(
            FetchAndLockRequest request,
            CancellationToken cancellationToken
        )
        {
            var externalTasks = await Client.FetchAndLockAsync(request, cancellationToken);
            return externalTasks;
        }

        protected virtual Task DelayOnFailure(CancellationToken cancellationToken) =>
            Task.Delay(10_000, cancellationToken);
    }
}
