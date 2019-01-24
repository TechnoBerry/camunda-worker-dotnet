// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class FailureResultTest
    {
        private readonly Mock<IExternalTaskCamundaClient> _clientMock = new Mock<IExternalTaskCamundaClient>();

        [Fact]
        public async Task TestExecuteResult()
        {
            var externalTask = new ExternalTask
            {
                Id = "testTask",
                WorkerId = "testWorker",
                TopicName = "testTopic",
                Variables = new Dictionary<string, Variable>()
            };

            ReportFailureRequest calledRequest = null;

            _clientMock
                .Setup(client =>
                    client.ReportFailure("testTask", It.IsAny<ReportFailureRequest>(), CancellationToken.None))
                .Callback((string taskId, ReportFailureRequest request, CancellationToken ct) =>
                {
                    calledRequest = request;
                })
                .Returns(Task.CompletedTask);

            var context = new ExternalTaskContext(externalTask, _clientMock.Object);

            var result = new FailureResult(new Exception("Message"));

            await result.ExecuteResult(context, CancellationToken.None);

            _clientMock.Verify(
                client => client.ReportFailure("testTask", It.IsAny<ReportFailureRequest>(), CancellationToken.None),
                Times.Once()
            );
            _clientMock.VerifyNoOtherCalls();

            Assert.NotNull(calledRequest);
            Assert.Equal("testWorker", calledRequest.WorkerId);
        }
    }
}
