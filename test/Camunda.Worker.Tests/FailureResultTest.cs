// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class FailureResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            _contextMock
                .Setup(context => context.ReportFailureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = new FailureResult(new Exception("Message"));

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.ReportFailureAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
