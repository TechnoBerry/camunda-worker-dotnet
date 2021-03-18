using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution
{
    public interface IExternalTaskSelector
    {
        Task<IReadOnlyCollection<ExternalTask>> SelectAsync(CancellationToken cancellationToken = default);
    }
}
