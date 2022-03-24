using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker;

public interface IExternalTaskHandler
{
    Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken);
}
