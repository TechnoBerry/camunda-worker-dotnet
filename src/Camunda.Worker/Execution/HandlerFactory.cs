using System;

namespace Camunda.Worker.Execution
{
    public delegate IExternalTaskHandler HandlerFactory(IServiceProvider provider);
}
