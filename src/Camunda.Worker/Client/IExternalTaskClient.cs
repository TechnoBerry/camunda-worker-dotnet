using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public interface IExternalTaskClient : IDisposable
    {
        Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken = default);

        Task Complete(string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default);

        Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default);

        Task ReportBpmnError(string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default);

        Task ExtendLock(string taskId, ExtendLockRequest request,
            CancellationToken cancellationToken = default);
    }
}
