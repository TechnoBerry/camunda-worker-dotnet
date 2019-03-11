// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public interface IExternalTaskCamundaClient
    {
        Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Task Complete(string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default(CancellationToken));

        Task ReportBpmnError(string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
