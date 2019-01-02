// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Api;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class BpmnErrorResultTest
    {
        private readonly Mock<ICamundaApiClient> _clientMock = new Mock<ICamundaApiClient>();

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

            BpmnErrorRequest calledRequest = null;

            _clientMock
                .Setup(client =>
                    client.ReportBpmnError("testTask", It.IsAny<BpmnErrorRequest>(), CancellationToken.None))
                .Callback((string taskId, BpmnErrorRequest request, CancellationToken ct) =>
                {
                    calledRequest = request;
                })
                .Returns(Task.CompletedTask);

            var context = new ExternalTaskContext(externalTask, _clientMock.Object);

            var result = new BpmnErrorResult("TEST_CODE", "Test message");

            await result.ExecuteResult(context, CancellationToken.None);

            _clientMock.Verify(
                client => client.ReportBpmnError("testTask", It.IsAny<BpmnErrorRequest>(), CancellationToken.None),
                Times.Once()
            );
            _clientMock.VerifyNoOtherCalls();

            Assert.NotNull(calledRequest);
            Assert.Equal("testWorker", calledRequest.WorkerId);
        }
    }
}
