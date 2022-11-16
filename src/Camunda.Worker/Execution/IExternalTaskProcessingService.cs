using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution;

internal interface IExternalTaskProcessingService
{
    Task ProcessAsync(ExternalTask externalTask, CancellationToken cancellationToken);
}
