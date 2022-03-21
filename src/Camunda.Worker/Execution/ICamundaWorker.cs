using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution;

public interface ICamundaWorker
{
    Task RunAsync(CancellationToken cancellationToken);
}
