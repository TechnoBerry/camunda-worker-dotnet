using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public interface IFetchAndLockRequestProvider
    {
        /// <summary>
        /// This method is called in the worker before each "fetch and lock" operation
        /// </summary>
        FetchAndLockRequest GetRequest();
    }
}
