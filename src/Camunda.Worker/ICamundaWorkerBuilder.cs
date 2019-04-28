using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface ICamundaWorkerBuilder
    {
        IServiceCollection Services { get; }

        ICamundaWorkerBuilder AddFactoryProvider<TProvider>() where TProvider : class, IHandlerFactoryProvider;

        ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider;

        ICamundaWorkerBuilder AddTaskSelector<TSelector>() where TSelector : class, IExternalTaskSelector;

        ICamundaWorkerBuilder AddExceptionHandler<THandler>() where THandler : class, IExceptionHandler;

        ICamundaWorkerBuilder AddHandlerDescriptor(HandlerDescriptor descriptor);
    }
}
