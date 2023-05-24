using System.Diagnostics.CodeAnalysis;
using Camunda.Worker.Variables;

namespace Camunda.Worker;

public static class ExternalTaskVariablesExtensions
{
    public static bool TryGetVariable<T>(
        this ExternalTask externalTask,
        string variableName,
        [NotNullWhen(true)] out T? variable
    ) where T : VariableBase
    {
        if (externalTask.Variables.TryGetValue(variableName, out var value))
        {
            if (value is T typedVariable)
            {
                variable = typedVariable;
                return true;
            }
        }

        variable = null;
        return false;
    }

    public static T? GetVariableOrDefault<T>(
        this ExternalTask externalTask,
        string variableName
    ) where T : VariableBase
    {
        if (externalTask.Variables.TryGetValue(variableName, out var value))
        {
            if (value is T typedVariable)
            {
                return typedVariable;
            }
        }

        return null;
    }
}
