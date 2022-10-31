namespace Camunda.Worker.Client;

public class ReportFailureRequest
{
    public ReportFailureRequest(string workerId)
    {
        WorkerId = Guard.NotNull(workerId, nameof(workerId));
    }

    public string WorkerId { get; }
    public string? ErrorMessage { get; set; }
    public string? ErrorDetails { get; set; }

    public int? Retries { get; set; }

    public int? RetryTimeout { get; set; }
}
