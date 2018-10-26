// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Api
{
    public interface ICamundaApiClient
    {
        Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request, CancellationToken cancellationToken);

        Task Complete(string taskId, CompleteRequest request, CancellationToken cancellationToken);

        Task ExtendLock(string taskId, ExtendLockRequest request, CancellationToken cancellationToken);
    }
}
