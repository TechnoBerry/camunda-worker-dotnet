// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Camunda.Worker.Api
{
    public interface ICamundaApiClient
    {
        [Post("/external-task/fetchAndLock")]
        Task<IList<ExternalTask>> FetchAndLock([Body] FetchAndLockRequest request, CancellationToken cancellationToken);

        [Post("/external-task/{taskId}/complete")]
        Task Complete(string taskId, [Body] CompleteRequest request, CancellationToken cancellationToken);

        [Post("/external-task/{taskId}/failure")]
        Task ReportFailure(string taskId, [Body] ReportFailureRequest request, CancellationToken cancellationToken);

        [Post("/external-task/{taskId}/extendLock")]
        Task ExtendLock(string taskId, [Body] ExtendLockRequest request, CancellationToken cancellationToken);
    }
}
