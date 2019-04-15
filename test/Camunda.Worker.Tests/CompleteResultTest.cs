#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CompleteResultTest
    {
        private readonly Mock<IExternalTaskContext> _contextMock = new Mock<IExternalTaskContext>();

        [Fact]
        public async Task TestExecuteResultAsync()
        {
            _contextMock
                .Setup(context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ))
                .Returns(Task.CompletedTask);

            var result = new CompleteResult(new Dictionary<string, Variable>());

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestExecuteResultWithFailedCompletion()
        {
            _contextMock
                .Setup(context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ))
                .ThrowsAsync(new ClientException(new ErrorResponse
                {
                    Type = "an error type",
                    Message = "an error message"
                }, HttpStatusCode.InternalServerError));

            _contextMock
                .Setup(context => context.ReportFailureAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = new CompleteResult(new Dictionary<string, Variable>());

            await result.ExecuteResultAsync(_contextMock.Object);

            _contextMock.Verify(
                context => context.CompleteAsync(
                    It.IsAny<IDictionary<string, Variable>>(),
                    It.IsAny<IDictionary<string, Variable>>()
                ),
                Times.Once()
            );
            _contextMock.Verify(
                context => context.ReportFailureAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once()
            );
            _contextMock.VerifyNoOtherCalls();
        }
    }
}
