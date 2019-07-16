using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface ICamundaWorkerBuilder
    {
        IServiceCollection Services { get; }

        ICamundaWorkerBuilder AddEndpointProvider<TProvider>() where TProvider : class, IEndpointProvider;

        ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider;

        ICamundaWorkerBuilder AddTaskSelector<TSelector>() where TSelector : class, IExternalTaskSelector;

        ICamundaWorkerBuilder AddHandlerDescriptor(HandlerDescriptor descriptor);

        ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction);
    }
}
