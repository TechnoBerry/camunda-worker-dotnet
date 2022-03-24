using System;
using System.Diagnostics.CodeAnalysis;

namespace Camunda.Worker;

internal static class Guard
{
    [ExcludeFromCodeCoverage]
    internal static T NotNull<T>(T parameterValue, string parameterName) where T : class
    {
        if (parameterValue == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return parameterValue;
    }

    [ExcludeFromCodeCoverage]
    internal static int GreaterThanOrEqual(int value, int minValue, string parameterName)
    {
        if (value < minValue)
        {
            throw new ArgumentException($"Must be greater than or equal to {minValue}", parameterName);
        }

        return value;
    }

    [ExcludeFromCodeCoverage]
    internal static string NotEmptyAndNotNull(string value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"Mustn't be null or empty string", parameterName);
        }

        return value;
    }
}
