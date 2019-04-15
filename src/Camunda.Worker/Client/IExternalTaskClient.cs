#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public interface IExternalTaskClient
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
