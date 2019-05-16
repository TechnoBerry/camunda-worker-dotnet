using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilder : ICamundaWorkerBuilder
    {
        public CamundaWorkerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public ICamundaWorkerBuilder AddFactoryProvider<TProvider>() where TProvider : class, IHandlerFactoryProvider
        {
            Services.AddTransient<IHandlerFactoryProvider, TProvider>();

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
    }
}
