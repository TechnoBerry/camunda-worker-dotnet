#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class BpmnErrorResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            _contextMock
                .Setup(context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ))
                .Returns(Task.CompletedTask);

            var result = new BpmnErrorResult("TEST_CODE", "Test message");

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.ReportBpmnErrorAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
