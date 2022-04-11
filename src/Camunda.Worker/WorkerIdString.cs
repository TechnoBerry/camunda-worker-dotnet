using System;

namespace Camunda.Worker;

/// <summary>
/// The type which represent a strongly typed valid worker's id
/// </summary>
public sealed class WorkerIdString : IEquatable<WorkerIdString>
{
    public WorkerIdString(string value)
    {
        Value = Guard.NotEmptyAndNotNull(value, nameof(value));
    }

    public string Value { get; }

    public bool Equals(WorkerIdString? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is WorkerIdString other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}
