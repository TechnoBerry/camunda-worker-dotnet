namespace Camunda.Worker.Execution
{
    public class PipelineDescriptor
    {
        public PipelineDescriptor(ExternalTaskDelegate externalTaskDelegate)
        {
            ExternalTaskDelegate = Guard.NotNull(externalTaskDelegate, nameof(externalTaskDelegate));
        }

        public ExternalTaskDelegate ExternalTaskDelegate { get; }
    }
}
