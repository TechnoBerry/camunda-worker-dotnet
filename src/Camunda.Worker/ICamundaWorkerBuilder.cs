using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface ICamundaWorkerBuilder
    {
        IServiceCollection Services { get; }

        string WorkerId { get; }

        ICamundaWorkerBuilder AddEndpointProvider<TProvider>() where TProvider : class, IEndpointProvider;

        ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider;

        ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, HandlerMetadata handlerMetadata);

        ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction);
    }
}
