using System;
using System.Threading;
using Camunda.Worker.Client;

namespace Camunda.Worker;

public interface IExternalTaskContext
{
    ExternalTask Task { get; }

    IExternalTaskClient Client { get; }

    IServiceProvider ServiceProvider { get; }

    CancellationToken ProcessingAborted { get; }
}
