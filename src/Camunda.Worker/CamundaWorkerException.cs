using System;
using System.Diagnostics.CodeAnalysis;

namespace Camunda.Worker;

[ExcludeFromCodeCoverage]
public class CamundaWorkerException : Exception
{
    public CamundaWorkerException(string? message) : base(message)
    {
    }

    public CamundaWorkerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
