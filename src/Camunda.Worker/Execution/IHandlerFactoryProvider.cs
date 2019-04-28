namespace Camunda.Worker.Execution
{
    public interface IHandlerFactoryProvider
    {
        HandlerFactory GetHandlerFactory(ExternalTask externalTask);
    }
}
