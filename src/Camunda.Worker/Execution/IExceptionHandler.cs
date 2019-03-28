#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;

namespace Camunda.Worker.Execution
{
    public interface IExceptionHandler
    {
        bool TryTransformToResult(Exception exception, out IExecutionResult executionResult);
    }
}
