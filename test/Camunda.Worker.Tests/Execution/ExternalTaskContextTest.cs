using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskContextTest
    {
        private readonly Mock<IExternalTaskClient> _clientMock = new();
        private readonly Mock<IServiceScope> _scopeMock = new();
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();

        public ExternalTaskContextTest()
        {
            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IExternalTaskClient)))
                .Returns(_clientMock.Object);
            _scopeMock.SetupGet(scope => scope.ServiceProvider)
                .Returns(_serviceProviderMock.Object);
        }

        [Fact]
        public async Task TestExtendLockAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.ExtendLockAsync(It.IsAny<string>(), It.IsNotNull<ExtendLockRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ExtendLockAsync(5_000);

            _clientMock.VerifyAll();
            Assert.False(context.Completed);
        }

        [Fact]
        public async Task TestCompleteAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.CompleteAsync(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.CompleteAsync(new Dictionary<string, Variable>());
            _clientMock.VerifyAll();
            Assert.True(context.Completed);
        }

        [Fact]
        public async Task TestReportFailureAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.ReportFailureAsync(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportFailureAsync("message", "details");

            _clientMock.VerifyAll();
            Assert.True(context.Completed);
        }

        [Fact]
        public async Task TestReportBpmnErrorAsync()
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.ReportBpmnErrorAsync(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            var context = CreateContext(externalTask);

            await context.ReportBpmnErrorAsync("code", "message");

            _clientMock.VerifyAll();
            Assert.True(context.Completed);
        }

        [Theory]
        [MemberData(nameof(GetDoubleCompletionArguments))]
        public async Task TestDoubleCompletion(Func<IExternalTaskContext, Task> first,
            Func<IExternalTaskContext, Task> second)
        {
            const string taskId = "testTask";
            var externalTask = CreateTask(taskId);

            _clientMock.Setup(client =>
                client.CompleteAsync(It.IsAny<string>(), It.IsNotNull<CompleteRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportFailureAsync(It.IsAny<string>(), It.IsNotNull<ReportFailureRequest>(), CancellationToken.None)
            ).Returns(Task.CompletedTask);

            _clientMock.Setup(client =>
                client.ReportBpmnErrorAsync(It.IsAny<string>(), It.IsNotNull<BpmnErrorRequest>(), CancellationToken.None)
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
            return new ExternalTaskContext(task, _serviceProviderMock.Object);
        }
    }
}
