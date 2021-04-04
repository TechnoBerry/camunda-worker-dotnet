namespace Camunda.Worker.Execution
{
    public class SelectorOptions
    {
        private int _asyncResponseTimeout = 10_000;

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set => _asyncResponseTimeout = Guard.GreaterThanOrEqual(value, 0, nameof(AsyncResponseTimeout));
        }

        public bool UsePriority { get; set; } = true;
    }
}
