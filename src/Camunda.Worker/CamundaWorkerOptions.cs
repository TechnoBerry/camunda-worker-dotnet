namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        private string _workerId = "camunda-worker";
        private int _asyncResponseTimeout = 10_000;

        public string WorkerId
        {
            get => _workerId;
            set => _workerId = Guard.NotNull(value, nameof(WorkerId));
        }

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set => _asyncResponseTimeout = Guard.GreaterThanOrEqual(value, 0, nameof(AsyncResponseTimeout));
        }
    }
}
