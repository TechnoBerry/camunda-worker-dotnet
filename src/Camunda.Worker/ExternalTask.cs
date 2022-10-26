using System.Collections.Generic;
using Camunda.Worker.Variables;

namespace Camunda.Worker;

public class ExternalTask
{
    public ExternalTask(string id, string workerId, string topicName)
    {
        Id = Guard.NotNull(id, nameof(id));
        WorkerId = Guard.NotNull(workerId, nameof(workerId));
        TopicName = Guard.NotNull(topicName, nameof(topicName));
    }

    /// <summary>
    /// The id of the external task.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The id of the worker that posesses or posessed the most recent lock.
    /// </summary>
    public string WorkerId { get; }

    /// <summary>
    /// The topic name of the external task.
    /// </summary>
    public string TopicName { get; }

    /// <summary>
    /// The priority of the external task
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// The id of the execution that the external task belongs to
    /// </summary>
    public string? ExecutionId { get; set; }

    /// <summary>
    /// The id of the activity that this external task belongs to
    /// </summary>
    public string? ActivityId { get; set; }

    /// <summary>
    /// The id of the activity instance that the external task belongs to
    /// </summary>
    public string? ActivityInstanceId { get; set; }

    /// <summary>
    /// The id of the tenant the external task belongs to
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// The id of the process definition the external task is defined in
    /// </summary>
    public string? ProcessDefinitionId { get; set; }

    /// <summary>
    /// The key of the process definition the external task is defined in
    /// </summary>
    public string? ProcessDefinitionKey { get; set; }

    /// <summary>
    /// The id of the process instance the external task belongs to
    /// </summary>
    public string? ProcessInstanceId { get; set; }

    /// <summary>
    /// The business key of the process instance the external task belongs to
    /// </summary>
    public string? BusinessKey { get; set; }

    /// <summary>
    /// The number of retries the task currently has left
    /// </summary>
    public int? Retries { get; set; }

    /// <summary>
    /// The full error message submitted with the latest reported failure executing this task
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The error details submitted with the latest reported failure executing this task
    /// </summary>
    public string? ErrorDetails { get; set; }

    public Dictionary<string, string>? ExtensionProperties { get; set; }

    public Dictionary<string, VariableBase> Variables { get; set; } = new();
}
