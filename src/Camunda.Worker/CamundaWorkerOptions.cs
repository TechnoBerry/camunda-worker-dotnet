namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        public CamundaWorkerOptions(string workerId)
        {
            WorkerId = Guard.NotEmptyAndNotNull(workerId, nameof(workerId));
        }

        public string WorkerId { get; }
    }
}
