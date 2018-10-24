// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public interface IExternalTaskExecutor
    {
        Task<IDictionary<string, Variable>> Execute(ExternalTask externalTask);
        Task<IDictionary<string, Variable>> Execute(ExternalTask externalTask, CancellationToken cancellationToken);
    }
}
