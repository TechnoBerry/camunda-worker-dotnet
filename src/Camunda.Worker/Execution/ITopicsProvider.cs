using System;
using System.Collections.Generic;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    [Obsolete("Use IFetchAndLockRequestProvider instead")]
    public interface ITopicsProvider
    {
        IReadOnlyCollection<FetchAndLockRequest.Topic> GetTopics();
    }
}
