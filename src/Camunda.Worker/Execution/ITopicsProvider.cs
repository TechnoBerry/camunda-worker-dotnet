using System.Collections.Generic;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public interface ITopicsProvider
    {
        IEnumerable<FetchAndLockRequest.Topic> GetTopics();
    }
}
