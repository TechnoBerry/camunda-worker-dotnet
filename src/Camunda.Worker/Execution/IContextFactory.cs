using System;

namespace Camunda.Worker.Execution
{
    public interface IContextFactory
    {
        IExternalTaskContext Create(ExternalTask externalTask, IServiceProvider serviceProvider);
    }
}
