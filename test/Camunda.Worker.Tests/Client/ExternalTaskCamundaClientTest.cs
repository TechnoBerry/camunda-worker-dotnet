#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace Camunda.Worker.Client
{
    public class ExternalTaskCamundaClientTest
    {
        private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();

        [Fact]
        public async Task TestFetchAndLock()
        {
            using (var client = MakeClient())
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

                var externalTasks = await client.FetchAndLock(request, CancellationToken.None);

                _handlerMock.VerifyNoOutstandingExpectation();
                var firstTask = Assert.Single(externalTasks);
                Assert.NotNull(firstTask);
            }
        }

        [Fact]
        public async Task TestComplete()
        {
            using (var client = MakeClient())
            {
                _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/complete")
                    .Respond(HttpStatusCode.NoContent);

                var request = new CompleteRequest("testWorker")
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["TEST"] = new Variable("testString")
                    }
                };

                await client.Complete("testTask", request, CancellationToken.None);

                _handlerMock.VerifyNoOutstandingExpectation();
            }
        }

        [Fact]
        public async Task TestReportFailure()
        {
            using (var client = MakeClient())
            {
                _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/failure")
                    .Respond(HttpStatusCode.NoContent);

                var request = new ReportFailureRequest("testWorker")
                {
                    ErrorMessage = "Error",
                    ErrorDetails = "Details"
                };

                await client.ReportFailure("testTask", request, CancellationToken.None);

                _handlerMock.VerifyNoOutstandingExpectation();
            }
        }

        [Fact]
        public async Task TestReportBpmnError()
        {
            using (var client = MakeClient())
            {
                _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/bpmnError")
                    .Respond(HttpStatusCode.NoContent);

                var request = new BpmnErrorRequest("testWorker", "testCode")
                {
                    ErrorMessage = "Error",
                    Variables = new Dictionary<string, Variable>()
                };

                await client.ReportBpmnError("testTask", request, CancellationToken.None);

                _handlerMock.VerifyNoOutstandingExpectation();
            }
        }

        [Fact]
        public async Task TestExtendLock()
        {
            using (var client = MakeClient())
            {
                _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/testTask/extendLock")
                    .Respond(HttpStatusCode.NoContent);

                var request = new ExtendLockRequest("testWorker", 10_000);

                await client.ExtendLock("testTask", request, CancellationToken.None);

                _handlerMock.VerifyNoOutstandingExpectation();
            }
        }

        [Theory]
        [MemberData(nameof(GetApiActions))]
        public async Task TestThrowsHttpRequestException(Func<IExternalTaskCamundaClient, Task> action)
        {
            using (var client = MakeClient())
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

                await Assert.ThrowsAsync<HttpRequestException>(() => action(client));
            }
        }

        [Theory]
        [MemberData(nameof(GetApiActions))]
        public async Task TestThrowsClientException(Func<IExternalTaskCamundaClient, Task> action)
        {
            using (var client = MakeClient())
            {
                var errorContent = new StringContent(@"
                    {
                        ""type"": ""An error type"",
                        ""message"": ""An error message""
                    }
                ");

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

                var clientException = await Assert.ThrowsAsync<ClientException>(() => action(client));
                Assert.Equal(HttpStatusCode.InternalServerError, clientException.StatusCode);
                Assert.Equal("An error type", clientException.ErrorType);
                Assert.Equal("An error message", clientException.ErrorMessage);
            }
        }

        public static IEnumerable<object[]> GetApiActions()
        {
            var fetchAndLockRequest = new FetchAndLockRequest("testWorker", 10);
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.FetchAndLock(fetchAndLockRequest))
            };

            var extendLockRequest = new ExtendLockRequest("testWorker", 10_000);
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ExtendLock("taskId", extendLockRequest))
            };

            var completeRequest = new CompleteRequest("testWorker");
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.Complete("taskId", completeRequest))
            };

            var reportFailureRequest = new ReportFailureRequest("test");
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportFailure("taskId", reportFailureRequest))
            };

            var bpmnErrorRequest = new BpmnErrorRequest("test", "test");
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportBpmnError("taskId", bpmnErrorRequest))
            };
        }

        private ExternalTaskCamundaClient MakeClient()
        {
            return new ExternalTaskCamundaClient(
                new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://test/api")
                }
            );
        }
    }
}
