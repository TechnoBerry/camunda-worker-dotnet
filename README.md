# Camunda.Worker

[![codecov](https://codecov.io/gh/AMalininHere/camunda-worker-dotnet/branch/master/graph/badge.svg)](https://codecov.io/gh/AMalininHere/camunda-worker-dotnet)
[![NuGet](https://img.shields.io/nuget/v/Camunda.Worker.svg)](https://www.nuget.org/packages/Camunda.Worker)

## Example

```csharp
[HandlerTopics("sayHello", LockDuration = 10_000)]
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
}
```
