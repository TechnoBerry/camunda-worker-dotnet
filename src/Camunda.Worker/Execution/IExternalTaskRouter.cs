using System.Threading.Tasks;

namespace Camunda.Worker.Execution
{
    public interface IExternalTaskRouter
    {
        Task RouteAsync(IExternalTaskContext context);
    }
}
