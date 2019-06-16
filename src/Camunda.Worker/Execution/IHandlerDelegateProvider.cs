namespace Camunda.Worker.Execution
{
    public interface IHandlerDelegateProvider
    {
        HandlerFactory GetHandlerFactory(ExternalTask externalTask);
    }
}
