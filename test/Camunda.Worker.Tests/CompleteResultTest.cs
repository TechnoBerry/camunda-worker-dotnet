// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CompleteResultTest
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

            CompleteRequest calledRequest = null;

            _clientMock
                .Setup(client => client.Complete("testTask", It.IsAny<CompleteRequest>(), CancellationToken.None))
                .Callback((string taskId, CompleteRequest request, CancellationToken ct) =>
                {
                    calledRequest = request;
                })
                .Returns(Task.CompletedTask);

            var context = new ExternalTaskContext(externalTask, _clientMock.Object);

            var result = new CompleteResult(new Dictionary<string, Variable>());

            await result.ExecuteResult(context);

            _clientMock.Verify(
                client => client.Complete("testTask", It.IsAny<CompleteRequest>(), CancellationToken.None),
                Times.Once()
            );
            _clientMock.VerifyNoOtherCalls();

            Assert.NotNull(calledRequest);
            Assert.Equal("testWorker", calledRequest.WorkerId);
        }
    }
}
