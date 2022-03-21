using System.Threading.Tasks;

namespace Camunda.Worker;

public class NoneResult : IExecutionResult
{
    public Task ExecuteResultAsync(IExternalTaskContext context) => Task.CompletedTask;
}
