using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution;

internal interface IExternalTaskProcessingService
{
    Task ProcessAsync(
        ExternalTask externalTask,
        IExternalTaskClient externalTaskClient,
        CancellationToken cancellationToken
    );
}
