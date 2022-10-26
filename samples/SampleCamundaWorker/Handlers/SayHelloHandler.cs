using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker;
using Camunda.Worker.Variables;

namespace SampleCamundaWorker.Handlers;

[HandlerTopics("sayHello", LockDuration = 10000)]
[HandlerVariables("USERNAME")]
public class SayHelloHandler : IExternalTaskHandler
{
    public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
    {
        if (!externalTask.TryGetVariable<StringVariable>("USERNAME", out var usernameVariable))
        {
            return new BpmnErrorResult("NO_USER", "Username not provided");
        }

        var username = usernameVariable.Value;

        await Task.Delay(1000, cancellationToken);

        return new CompleteResult
        {
            Variables = new Dictionary<string, VariableBase>
            {
                ["MESSAGE"] = new StringVariable($"Hello, {username}!"),
                ["USER_INFO"] = JsonVariable.Create(new UserInfo(username, new List<string>
                {
                    "Admin"
                }))
            }
        };
    }

    public class UserInfo
    {
        public UserInfo(string username, List<string> roles)
        {
            Username = username;
            Roles = roles;
        }

        public string Username { get; }

        public List<string> Roles { get; }
    }
}
