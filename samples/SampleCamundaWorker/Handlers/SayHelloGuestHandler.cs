using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopics("sayHelloGuest")]
    public class SayHelloGuestHandler : ExternalTaskHandler
    {
        public override Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            return Task.FromResult<IExecutionResult>(new CompleteResult(new Dictionary<string, Variable>
            {
                ["MESSAGE"] = Variable.String("Hello, Guest!")
            }));
        }
    }
}
