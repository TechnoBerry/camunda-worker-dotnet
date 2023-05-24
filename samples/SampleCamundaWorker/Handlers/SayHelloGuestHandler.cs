using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker;
using Camunda.Worker.Variables;

namespace SampleCamundaWorker.Handlers;

[HandlerTopics("sayHelloGuest")]
public class SayHelloGuestHandler : IExternalTaskHandler
{
    public Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
    {
        return Task.FromResult<IExecutionResult>(new CompleteResult
        {
            Variables = new Dictionary<string, VariableBase>
            {
                ["MESSAGE"] = new StringVariable("Hello, Guest!")
            }
        });
    }
}
