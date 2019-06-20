namespace Camunda.Worker.Execution
{
    public interface IHandlerDelegateProvider
    {
        ExternalTaskDelegate GetHandlerDelegate(ExternalTask externalTask);
    }
}
