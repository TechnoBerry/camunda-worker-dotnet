// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Core
{
    public interface IExternalTaskExecutor
    {
        Task<ExecutionResult> Execute(ExternalTask externalTask);
        Task<ExecutionResult> Execute(ExternalTask externalTask, CancellationToken cancellationToken);
    }
}
