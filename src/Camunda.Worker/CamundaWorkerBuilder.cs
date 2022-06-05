using System;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Camunda.Worker;

public class CamundaWorkerBuilder : ICamundaWorkerBuilder
{
    public CamundaWorkerBuilder(IServiceCollection services, WorkerIdString workerId)
    {
        Services = services;
        WorkerId = workerId;
    }

    public IServiceCollection Services { get; }

    public WorkerIdString WorkerId { get; }

    public ICamundaWorkerBuilder AddEndpointProvider<TProvider>()
        where TProvider : class, IEndpointProvider
    {
        Services.AddSingleton<IEndpointProvider, TProvider>();
        return this;
    }

    internal CamundaWorkerBuilder AddDefaultFetchAndLockRequestProvider()
    {
        AddFetchAndLockRequestProvider((workerId, provider) => new FetchAndLockRequestProvider(
            workerId,
            provider.GetRequiredService<IOptionsMonitor<FetchAndLockOptions>>(),
            provider.GetServices<HandlerDescriptor>()
        ));

        return this;
    }

    public ICamundaWorkerBuilder AddFetchAndLockRequestProvider(
        Func<WorkerIdString, IServiceProvider, IFetchAndLockRequestProvider> factory
    )
    {
        Services.AddSingleton(provider => factory(WorkerId, provider));

        return this;
    }

    public ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, HandlerMetadata handlerMetadata)
    {
        Guard.NotNull(handler, nameof(handler));
        Guard.NotNull(handlerMetadata, nameof(handlerMetadata));

        var descriptor = new HandlerDescriptor(handler, handlerMetadata, WorkerId);

        Services.AddSingleton(descriptor);
        return this;
    }

    public ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction)
    {
        Guard.NotNull(configureAction, nameof(configureAction));
        Services.AddSingleton(provider =>
        {
            var externalTaskDelegate = new PipelineBuilder(provider, WorkerId)
                .Also(configureAction)
                .Build(ExternalTaskRouter.RouteAsync);
            return new WorkerHandlerDescriptor(externalTaskDelegate);
        });
        return this;
    }

    public ICamundaWorkerBuilder ConfigureEvents(Action<WorkerEvents> configureAction)
    {
        Services.Configure(configureAction);
        return this;
    }
}
