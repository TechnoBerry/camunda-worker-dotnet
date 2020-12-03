using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Camunda.Worker.Client
{
    public class ExternalTaskCamundaTest : IDisposable
    {
        private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();
        private readonly ExternalTaskClient _client;

        public ExternalTaskCamundaTest()
        {
            _client = new ExternalTaskClient(
                new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://test/api")
                }
            );
        }

        public void Dispose()
        {
            _handlerMock?.Dispose();
        }

        [Fact]
        public async Task TestFetchAndLock()
        {
            _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/fetchAndLock")
                .Respond("application/json", @"[
                        {
                            ""id"": ""testTask"",
                            ""workerId"": ""testWorker"",
                            ""topicName"": ""testTopic"",
                            ""processDefinitionId"": ""testDefinitionId"",
                            ""processDefinitionKey"": ""testDefinitionKey"",
                            ""activityId"": ""anActivityId"",
                            ""activityInstanceId"": ""anActivityInstanceId"",
                            ""errorMessage"": ""anErrorMessage"",
                            ""errorDetails"": ""anErrorDetails"",
                            ""executionId"": ""anExecutionId"",
                            ""tenantId"": null,
                            ""retries"": 3,
                            ""priority"": 4,
                            ""variables"": {
                                ""TEST"": {
                                    ""value"": ""testString"",
                                    ""type"": ""String""
                                }
                            }
                        }
                    ]");

            var request = new FetchAndLockRequest("testWorker", 10)
            {
                AsyncResponseTimeout = 10_000,
                Topics = new[]
                {
                    new FetchAndLockRequest.Topic("testTopic", 10_000)
                }
            };

            var externalTasks = await _client.FetchAndLockAsync(request, CancellationToken.None);

            _handlerMock.VerifyNoOutstandingExpectation();
            var firstTask = Assert.Single(externalTasks);
            Assert.NotNull(firstTask);
        }

        [Fact]
        public async Task TestComplete()
        {
            _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/complete")
                .Respond(HttpStatusCode.NoContent);

            var request = new CompleteRequest("testWorker")
            {
                Variables = new Dictionary<string, Variable>
                {
                    ["TEST"] = Variable.String("testString")
                }
            };

            await _client.CompleteAsync("testTask", request, CancellationToken.None);

            _handlerMock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task TestReportFailure()
        {
            _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/failure")
                .Respond(HttpStatusCode.NoContent);

            var request = new ReportFailureRequest("testWorker")
            {
                ErrorMessage = "Error",
                ErrorDetails = "Details"
            };

            await _client.ReportFailureAsync("testTask", request, CancellationToken.None);

            _handlerMock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task TestReportBpmnError()
        {
            _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/bpmnError")
                .Respond(HttpStatusCode.NoContent);

            var request = new BpmnErrorRequest("testWorker", "testCode", "Error")
            {
                Variables = new Dictionary<string, Variable>()
            };

            await _client.ReportBpmnErrorAsync("testTask", request, CancellationToken.None);

            _handlerMock.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task TestExtendLock()
        {
            _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/extendLock")
                .Respond(HttpStatusCode.NoContent);

            var request = new ExtendLockRequest("testWorker", 10_000);

            await _client.ExtendLockAsync("testTask", request, CancellationToken.None);

            _handlerMock.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [MemberData(nameof(GetApiActions))]
        public async Task TestThrowsHttpRequestException(Func<IExternalTaskClient, Task> action)
        {
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/fetchAndLock")
                .Respond(HttpStatusCode.InternalServerError);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/extendLock")
                .Respond(HttpStatusCode.InternalServerError);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/complete")
                .Respond(HttpStatusCode.InternalServerError);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/failure")
                .Respond(HttpStatusCode.InternalServerError);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/bpmnError")
                .Respond(HttpStatusCode.InternalServerError);

            await Assert.ThrowsAsync<HttpRequestException>(() => action(_client));
        }

        [Theory]
        [MemberData(nameof(GetApiActions))]
        public async Task TestThrowsClientException(Func<IExternalTaskClient, Task> action)
        {
            var errorContent = new StringContent(@"
                {
                    ""type"": ""An error type"",
                    ""message"": ""An error message""
                }
            ", Encoding.UTF8, "application/json");

            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/fetchAndLock")
                .Respond(HttpStatusCode.InternalServerError, errorContent);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/extendLock")
                .Respond(HttpStatusCode.InternalServerError, errorContent);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/complete")
                .Respond(HttpStatusCode.InternalServerError, errorContent);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/failure")
                .Respond(HttpStatusCode.InternalServerError, errorContent);
            _handlerMock.When(HttpMethod.Post, "http://test/api/external-task/taskId/bpmnError")
                .Respond(HttpStatusCode.InternalServerError, errorContent);

            var clientException = await Assert.ThrowsAsync<ClientException>(() => action(_client));
            Assert.Equal(HttpStatusCode.InternalServerError, clientException.StatusCode);
            Assert.Equal("An error type", clientException.ErrorType);
            Assert.Equal("An error message", clientException.ErrorMessage);
        }

        public static IEnumerable<object[]> GetApiActions()
        {
            var fetchAndLockRequest = new FetchAndLockRequest("testWorker", 10);
            yield return new object[]
            {
                new Func<IExternalTaskClient, Task>(c => c.FetchAndLockAsync(fetchAndLockRequest))
            };

            var extendLockRequest = new ExtendLockRequest("testWorker", 10_000);
            yield return new object[]
            {
                new Func<IExternalTaskClient, Task>(c => c.ExtendLockAsync("taskId", extendLockRequest))
            };

            var completeRequest = new CompleteRequest("testWorker");
            yield return new object[]
            {
                new Func<IExternalTaskClient, Task>(c => c.CompleteAsync("taskId", completeRequest))
            };

            var reportFailureRequest = new ReportFailureRequest("test");
            yield return new object[]
            {
                new Func<IExternalTaskClient, Task>(c => c.ReportFailureAsync("taskId", reportFailureRequest))
            };

            var bpmnErrorRequest = new BpmnErrorRequest("test", "test", "test");
            yield return new object[]
            {
                new Func<IExternalTaskClient, Task>(c => c.ReportBpmnErrorAsync("taskId", bpmnErrorRequest))
            };
        }

        private ExternalTaskClient MakeClient()
        {
            return new ExternalTaskClient(
                new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://test/api")
                }
            );
        }
    }
}
