# Camunda.Worker

[![codecov](https://codecov.io/gh/AMalininHere/camunda-worker-dotnet/branch/master/graph/badge.svg)](https://codecov.io/gh/AMalininHere/camunda-worker-dotnet)
[![NuGet](https://img.shields.io/nuget/v/Camunda.Worker.svg)](https://www.nuget.org/packages/Camunda.Worker)

## Example

```csharp
[HandlerTopics("sayHello", LockDuration = 10_000)]
[HandlerVariables("USERNAME")]
public class SayHelloHandler : IExternalTaskHandler
{
    public async Task<IExecutionResult> HandleAsync(ExternalTask externalTask)
    {
        var username = externalTask.Variables["USERNAME"].Value;

        await Task.Delay(1000);

        return new CompleteResult(new Dictionary<string, Variable>
        {
            ["MESSAGE"] = Variable.String($"Hello, {username}!")
        });
    }
}
```
