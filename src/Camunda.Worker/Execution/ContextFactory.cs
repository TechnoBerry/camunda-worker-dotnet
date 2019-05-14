using System;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution
{
    public class ContextFactory : IContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IExternalTaskContext MakeContext(ExternalTask externalTask) =>
            new ExternalTaskContext(externalTask, _serviceProvider.CreateScope());
    }
}
