using System;
using System.Threading;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution;

public sealed class ExternalTaskContext : IExternalTaskContext
{
    public ExternalTaskContext(
        ExternalTask task,
        IExternalTaskClient client,
        IServiceProvider provider,
        CancellationToken processingAborted = default
    )
    {
        Task = Guard.NotNull(task, nameof(task));
        Client = Guard.NotNull(client, nameof(client));
        ServiceProvider = Guard.NotNull(provider, nameof(provider));
        ProcessingAborted = processingAborted;
    }

    public ExternalTask Task { get; }

    public IExternalTaskClient Client { get; }

    public IServiceProvider ServiceProvider { get; }

    public CancellationToken ProcessingAborted { get; }
}
