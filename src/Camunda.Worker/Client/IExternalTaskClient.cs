using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public interface IExternalTaskClient : IDisposable
    {
        Task<List<ExternalTask>> FetchAndLockAsync(
            FetchAndLockRequest request,
            CancellationToken cancellationToken = default
        );

        Task CompleteAsync(
            string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default
        );

        Task ReportFailureAsync(
            string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default
        );

        Task ReportBpmnErrorAsync(
            string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default
        );

        Task ExtendLockAsync(
            string taskId, ExtendLockRequest request,
            CancellationToken cancellationToken = default
        );
    }
}
