using System;

namespace Camunda.Worker.Execution
{
    public class ContextFactory : IContextFactory
    {
        public IExternalTaskContext Create(ExternalTask externalTask, IServiceProvider serviceProvider)
            => new ExternalTaskContext(externalTask, serviceProvider);
    }
}
