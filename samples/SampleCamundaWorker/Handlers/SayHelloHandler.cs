using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopics("sayHello", LockDuration = 10000)]
    [HandlerVariables("USERNAME")]
    public class SayHelloHandler : IExternalTaskHandler
    {
        public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask)
        {
            if (!externalTask.Variables.TryGetValue("USERNAME", out var usernameVariable))
            {
                return new BpmnErrorResult("NO_USER", "Username not provided");
            }

            var username = usernameVariable.Value;

            await Task.Delay(1000);

            return new CompleteResult
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["MESSAGE"] = Variable.String($"Hello, {username}!")
                }
            };
        }
    }
}
