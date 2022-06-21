using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker;

public interface ICamundaWorkerBuilder
{
    IServiceCollection Services { get; }

    WorkerIdString WorkerId { get; }

    ICamundaWorkerBuilder AddEndpointProvider<TProvider>() where TProvider : class, IEndpointProvider;

    ICamundaWorkerBuilder AddFetchAndLockRequestProvider(
        Func<WorkerIdString, IServiceProvider, IFetchAndLockRequestProvider> factory
    );

    ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, HandlerMetadata handlerMetadata);

    ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction);

    ICamundaWorkerBuilder ConfigureEvents(Action<WorkerEvents> configureAction);
}
