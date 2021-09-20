namespace Camunda.Worker.Execution
{
    public class FetchAndLockOptions
    {
        private int _maxTasks = 1;
        private int _asyncResponseTimeout = 10_000;

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
