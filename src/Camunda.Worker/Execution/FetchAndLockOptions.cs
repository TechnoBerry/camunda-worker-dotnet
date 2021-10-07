namespace Camunda.Worker.Execution
{
    public class FetchAndLockOptions
    {
        private string _workerId = string.Empty;
        private int _maxTasks = 1;
        private int _asyncResponseTimeout = 10_000;

        public string WorkerId
        {
            get => _workerId;
            internal set => _workerId = Guard.NotEmptyAndNotNull(value, nameof(WorkerId));
        }

        public int MaxTasks
        {
            get => _maxTasks;
            set => _maxTasks = Guard.GreaterThanOrEqual(value, 1, nameof(MaxTasks));
        }

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set => _asyncResponseTimeout = Guard.GreaterThanOrEqual(value, 0, nameof(AsyncResponseTimeout));
        }

        public bool UsePriority { get; set; } = true;
    }
}
