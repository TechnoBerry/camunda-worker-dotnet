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

        [Obsolete("Use IFetchAndLockRequestProvider and AddFetchAndLockRequestProvider instead")]
        ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider;

        ICamundaWorkerBuilder AddFetchAndLockRequestProvider(
            Func<WorkerServiceOptions, IServiceProvider, IFetchAndLockRequestProvider> factory
        );

        ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, HandlerMetadata handlerMetadata);

        ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction);

        ICamundaWorkerBuilder ConfigureEvents(Action<WorkerEvents> configureAction);
    }
}
