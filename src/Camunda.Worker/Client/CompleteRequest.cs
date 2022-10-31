using System.Collections.Generic;
using Camunda.Worker.Variables;

namespace Camunda.Worker.Client;

public class CompleteRequest
{
    public CompleteRequest(string workerId)
    {
        WorkerId = Guard.NotNull(workerId, nameof(workerId));
    }

    public string WorkerId { get; }

    public Dictionary<string, VariableBase>? Variables { get; set; }

    public Dictionary<string, VariableBase>? LocalVariables { get; set; }
}
