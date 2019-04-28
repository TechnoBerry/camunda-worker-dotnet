using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public interface IExternalTaskSelector
    {
        Task<IEnumerable<ExternalTask>> SelectAsync(IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default);
    }
}
