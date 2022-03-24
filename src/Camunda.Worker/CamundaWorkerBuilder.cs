using System;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker;

public class CamundaWorkerBuilder : ICamundaWorkerBuilder
{
    public CamundaWorkerBuilder(IServiceCollection services, string workerId)
    {
        Services = services;
        WorkerId = workerId;
    }

    public IServiceCollection Services { get; }

    public string WorkerId { get; }

    public ICamundaWorkerBuilder AddEndpointProvider<TProvider>()
        where TProvider : class, IEndpointProvider
    {
        Services.AddSingleton<IEndpointProvider, TProvider>();
        return this;
    }

    public ICamundaWorkerBuilder AddTopicsProvider<TProvider>() where TProvider : class, ITopicsProvider
    {
        Services.AddTransient<ITopicsProvider, TProvider>();
        return this;
    }

    internal CamundaWorkerBuilder AddFetchAndLockRequestProvider(
        Func<WorkerServiceOptions, IServiceProvider, IFetchAndLockRequestProvider> factory
    )
    {
        Services.AddSingleton(provider =>
        {
            var handlerDescriptors = provider.GetServices<HandlerDescriptor>();
            var options = new WorkerServiceOptions(WorkerId, handlerDescriptors);
            return factory(options, provider);
        });

        return this;
    }

    public ICamundaWorkerBuilder AddHandler(ExternalTaskDelegate handler, HandlerMetadata handlerMetadata)
    {
        Guard.NotNull(handler, nameof(handler));
        Guard.NotNull(handlerMetadata, nameof(handlerMetadata));

        var descriptor = new HandlerDescriptor(handler, handlerMetadata);

        Services.AddSingleton(descriptor);
        return this;
    }

    public ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction)
    {
        Guard.NotNull(configureAction, nameof(configureAction));
        Services.AddSingleton(provider =>
        {
            var externalTaskDelegate = new PipelineBuilder(provider)
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
