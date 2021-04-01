namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        private string _workerId = "camunda-worker";

        public string WorkerId
        {
            get => _workerId;
            set => _workerId = Guard.NotNull(value, nameof(WorkerId));
        }
    }
}
