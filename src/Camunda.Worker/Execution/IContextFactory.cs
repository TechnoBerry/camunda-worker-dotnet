namespace Camunda.Worker.Execution
{
    public interface IContextFactory
    {
        IExternalTaskContext MakeContext(ExternalTask externalTask);
    }
}
