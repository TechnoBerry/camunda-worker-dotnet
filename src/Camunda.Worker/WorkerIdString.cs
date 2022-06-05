using System;
using System.Diagnostics.CodeAnalysis;

namespace Camunda.Worker;

public readonly struct WorkerIdString : IEquatable<WorkerIdString>
{
    private const string DefaultValue = "camunda-worker-dotnet";

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public WorkerIdString()
    {
        Value = DefaultValue;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public WorkerIdString(string value)
    {
        Value = Guard.NotEmptyAndNotNull(value, nameof(value));
    }

    public string Value { get; }

    public static implicit operator WorkerIdString(string value) => new(value);

    public bool Equals(WorkerIdString other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is WorkerIdString other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(WorkerIdString left, WorkerIdString right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WorkerIdString left, WorkerIdString right)
    {
        return !(left == right);
    }
}
