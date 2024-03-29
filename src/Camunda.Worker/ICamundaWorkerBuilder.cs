using System;
using Camunda.Worker.Endpoints;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker;

public interface ICamundaWorkerBuilder
{
    IServiceCollection Services { get; }

    WorkerIdString WorkerId { get; }

    ICamundaWorkerBuilder AddEndpointResolver(WorkerServiceFactory<IEndpointResolver> factory);

    ICamundaWorkerBuilder AddFetchAndLockRequestProvider(WorkerServiceFactory<IFetchAndLockRequestProvider> factory);

    ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, EndpointMetadata endpointMetadata);

    ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction);

    ICamundaWorkerBuilder ConfigureEvents(Action<WorkerEvents> configureAction);
}
