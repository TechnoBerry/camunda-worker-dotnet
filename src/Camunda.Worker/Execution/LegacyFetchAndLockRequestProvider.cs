using Camunda.Worker.Client;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution;

internal class LegacyFetchAndLockRequestProvider : IFetchAndLockRequestProvider
{
    private readonly WorkerIdString _workerId;
    private readonly ITopicsProvider _topicsProvider;
    private readonly FetchAndLockOptions _options;

    internal LegacyFetchAndLockRequestProvider(
        WorkerIdString workerId,
        ITopicsProvider topicsProvider,
        IOptions<FetchAndLockOptions> options
    )
    {
        _workerId = workerId;
        _topicsProvider = topicsProvider;
        _options = options.Value;
    }

    public FetchAndLockRequest GetRequest()
    {
        var topics = _topicsProvider.GetTopics();

        var fetchAndLockRequest = new FetchAndLockRequest(_workerId.Value, _options.MaxTasks)
        {
            UsePriority = _options.UsePriority,
            AsyncResponseTimeout = _options.AsyncResponseTimeout,
            Topics = topics
        };

        return fetchAndLockRequest;
    }
}
