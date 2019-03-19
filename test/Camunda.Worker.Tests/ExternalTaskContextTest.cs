#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
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

            _clientMock.Setup(client =>
                client.Complete(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.CompleteAsync(new Dictionary<string, Variable>());
            _clientMock.VerifyAll();
        }

        [Fact]
        public async Task TestReportFailureAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.ReportFailure(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportFailureAsync("message", "details");

            _clientMock.VerifyAll();
        }

        [Fact]
        public async Task TestReportBpmnErrorAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.ReportBpmnError(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportBpmnErrorAsync("code", "message");

            _clientMock.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(GetDoubleCompletionArguments))]
        public async Task TestDoubleCompletion(Func<IExternalTaskContext, Task> first,
            Func<IExternalTaskContext, Task> second)
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.Complete(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportFailure(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportBpmnError(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await first(context);
            await Assert.ThrowsAsync<CamundaWorkerException>(() => second(context));
        }

        public static IEnumerable<object[]> GetDoubleCompletionArguments()
        {
            return GetCompletionFunctions()
                .Join(GetCompletionFunctions(), _ => true, _ => true, (a, b) => new object[] {a, b});
        }

        private static IEnumerable<Func<IExternalTaskContext, Task>> GetCompletionFunctions()
        {
            yield return ctx => ctx.CompleteAsync(new Dictionary<string, Variable>());
            yield return ctx => ctx.ReportFailureAsync("message", "details");
            yield return ctx => ctx.ReportBpmnErrorAsync("core", "message");
        }

        private static ExternalTask CreateTask(string id)
        {
            return new ExternalTask(id, "testWorker", "testTopic")
            {
                Variables = new Dictionary<string, Variable>()
            };
        }

        private IExternalTaskContext CreateContext(ExternalTask task)
        {
            return new ExternalTaskContext(task, _clientMock.Object);
        }
    }
}
