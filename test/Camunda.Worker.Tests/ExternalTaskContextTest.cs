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
    public class ExternalTaskContextTest
    {
        private readonly Mock<IExternalTaskCamundaClient> _clientMock = new Mock<IExternalTaskCamundaClient>();

        [Fact]
        public async Task TestCompleteAsync()
        {
            var externalTask = new ExternalTask
            {
                Id = "testTask",
                WorkerId = "testWorker",
                TopicName = "testTopic",
                Variables = new Dictionary<string, Variable>()
            };

            _clientMock
                .Setup(client => client.Complete("testTask", It.IsNotNull<CompleteRequest>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            var context = Create(externalTask);

            await context.CompleteAsync(new Dictionary<string, Variable>());

            _clientMock.Verify(
                client => client.Complete("testTask", It.IsAny<CompleteRequest>(), CancellationToken.None),
                Times.Once()
            );
            _clientMock.VerifyNoOtherCalls();
        }

        private IExternalTaskContext Create(ExternalTask task)
        {
            return new ExternalTaskContext(task, _clientMock.Object);
        }
    }
}
