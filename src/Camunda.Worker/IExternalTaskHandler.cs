using System.Threading.Tasks;

namespace Camunda.Worker
{
    public interface IExternalTaskHandler
    {
        Task HandleAsync(IExternalTaskContext context);
        Task<IExecutionResult> Process(ExternalTask externalTask);
    }
}
