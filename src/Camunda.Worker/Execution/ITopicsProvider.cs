using System.Collections.Generic;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution;

public interface ITopicsProvider
{
    IReadOnlyCollection<FetchAndLockRequest.Topic> GetTopics();
}
