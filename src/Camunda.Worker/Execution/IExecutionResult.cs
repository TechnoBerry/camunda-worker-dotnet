// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Execution
{
    public interface IExecutionResult
    {
        Task ExecuteResult(ExternalTaskContext context, CancellationToken cancellationToken);
    }
}
