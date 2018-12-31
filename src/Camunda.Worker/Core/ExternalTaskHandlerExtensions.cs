// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Core
{
    internal static class ExternalTaskHandlerExtensions
    {
        internal static async Task<IExecutionResult> ProcessSafe(this IExternalTaskHandler handler,
            ExternalTask externalTask, CancellationToken cancellationToken)
        {
            try
            {
                var result = await handler.Process(externalTask, cancellationToken);
                return result;
            }
            catch (Exception e)
            {
                return new FailureResult(e);
            }
        }
    }
}
