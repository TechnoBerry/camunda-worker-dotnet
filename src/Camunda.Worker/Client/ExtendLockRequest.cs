namespace Camunda.Worker.Client
{
    public class ExtendLockRequest
    {
        public ExtendLockRequest(string workerId, int newDuration)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            NewDuration = newDuration;
        }

        public string WorkerId { get; }
        public int NewDuration { get; }
    }
}
