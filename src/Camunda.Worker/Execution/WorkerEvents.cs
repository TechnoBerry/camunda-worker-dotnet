using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution;

[ExcludeFromCodeCoverage]
public class WorkerEvents
{
    public Func<IServiceProvider, CancellationToken, Task> OnBeforeFetchAndLock { get; set; } =
        DefaultOnBeforeFetchAndLock;

    public Func<IServiceProvider, CancellationToken, Task> OnFailedFetchAndLock { get; set; } =
        DefaultOnFailedFetchAndLock;

    public Func<IServiceProvider, CancellationToken, Task> OnAfterProcessingAllTasks { get; set; } =
        DefaultOnAfterProcessingAllTasks;

    private static Task DefaultOnBeforeFetchAndLock(IServiceProvider provider, CancellationToken ct) =>
        Task.CompletedTask;

    private static Task DefaultOnFailedFetchAndLock(IServiceProvider provider, CancellationToken ct) =>
        Task.Delay(10_000, ct);

    private static Task DefaultOnAfterProcessingAllTasks(IServiceProvider provider, CancellationToken ct) =>
        Task.CompletedTask;
}
