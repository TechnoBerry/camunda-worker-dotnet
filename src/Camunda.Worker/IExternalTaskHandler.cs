using System.Threading.Tasks;

namespace Camunda.Worker
{
    public interface IExternalTaskHandler
    {
        Task<IExecutionResult> Process(ExternalTask externalTask);
    }
}
