#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultExceptionHandlerTest
    {
        private readonly IExceptionHandler _exceptionHandler = new DefaultExceptionHandler();

        [Fact]
        public void TestTransformExceptionToFailureResult()
        {
            var exception = new Exception();

            var transformResult = _exceptionHandler.TryTransformToResult(exception, out var executionResult);

            Assert.True(transformResult);
            Assert.IsType<FailureResult>(executionResult);
        }
    }
}
