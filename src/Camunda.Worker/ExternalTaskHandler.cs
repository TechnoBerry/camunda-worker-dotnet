using System.Threading.Tasks;

namespace Camunda.Worker
{
    public abstract class ExternalTaskHandler : IExternalTaskHandler
    {
        public abstract Task<IExecutionResult> Process(ExternalTask externalTask);
    }
}
