// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            Expression<Func<IExternalTaskCamundaClient, Task>> expression =
                client => client.Complete(taskId, It.IsNotNull<CompleteRequest>(), CancellationToken.None);

            _clientMock.Setup(expression).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.CompleteAsync(new Dictionary<string, Variable>());

            _clientMock.Verify(expression, Times.Once());
            _clientMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestReportFailureAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            Expression<Func<IExternalTaskCamundaClient, Task>> expression =
                client => client.ReportFailure(taskId, It.IsNotNull<ReportFailureRequest>(), CancellationToken.None);

            _clientMock.Setup(expression).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportFailureAsync("message", "details");

            _clientMock.Verify(expression, Times.Once());
            _clientMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task TestReportBpmnErrorAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            Expression<Func<IExternalTaskCamundaClient, Task>> expression =
                client => client.ReportBpmnError(taskId, It.IsNotNull<BpmnErrorRequest>(), CancellationToken.None);

            _clientMock.Setup(expression).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportBpmnErrorAsync("code", "message");

            _clientMock.Verify(expression, Times.Once());
            _clientMock.VerifyNoOtherCalls();
        }

        private static ExternalTask CreateTask(string id)
        {
            return new ExternalTask
            {
                Id = id,
                WorkerId = "testWorker",
                TopicName = "testTopic",
                Variables = new Dictionary<string, Variable>()
            };
        }

        private IExternalTaskContext CreateContext(ExternalTask task)
        {
            return new ExternalTaskContext(task, _clientMock.Object);
        }
    }
}
