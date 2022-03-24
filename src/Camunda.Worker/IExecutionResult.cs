using System.Threading.Tasks;

namespace Camunda.Worker;

/// <summary>
/// Defines a contract that represents the result of external task's handler
/// </summary>
public interface IExecutionResult
{
    /// <summary>
    /// Executes the result operation of the external task's handler asynchronously
    /// </summary>
    /// <param name="context">The context in which the result is executed</param>
    /// <returns>A task that represents the asynchronous execute operation.</returns>
    Task ExecuteResultAsync(IExternalTaskContext context);
}
