using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
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

        public ICamundaWorkerBuilder AddTaskSelector<TSelector>() where TSelector : class, IExternalTaskSelector
        {
            Services.AddTransient<IExternalTaskSelector, TSelector>();
            return this;
        }

        public ICamundaWorkerBuilder AddHandlerDescriptor(HandlerDescriptor descriptor)
        {
            Guard.NotNull(descriptor, nameof(descriptor));
            Services.AddSingleton(descriptor);
            return this;
        }

        public ICamundaWorkerBuilder ConfigurePipeline(Action<IPipelineBuilder> configureAction)
        {
            Guard.NotNull(configureAction, nameof(configureAction));
            var builder = new PipelineBuilder(Services);
            configureAction(builder);
            var externalTaskDelegate = builder.Build(PipelineBuilder.RouteAsync);
            Services.AddSingleton(new PipelineDescriptor(externalTaskDelegate));
            return this;
        }
    }
}
