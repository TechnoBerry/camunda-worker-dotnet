// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
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
        public void TestConstructWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExternalTaskCamundaClient(null));
        }

        [Theory]
        [MemberData(nameof(GetActionsWithNullArgument))]
        public async Task TestCallWithNullArgument(Func<IExternalTaskCamundaClient, Task> func)
        {
            using (var client = MakeClient())
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() => func(client));
            }
        }

        public static IEnumerable<object[]> GetActionsWithNullArgument()
        {
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.FetchAndLock(null))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.Complete("taskId", null))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.Complete(null, new CompleteRequest("test")))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportFailure("taskId", null))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportFailure(null, new ReportFailureRequest("test")))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportFailure("taskId", null))
            };
            yield return new object[]
            {
                new Func<IExternalTaskCamundaClient, Task>(c => c.ReportFailure(null, new ReportFailureRequest("test")))
            };
        }

        [Fact]
        public async Task TestFetchAndLock()
        {
            using (var client = MakeClient())
            {
                _handlerMock.Expect(HttpMethod.Post, "http://test/api/external-task/fetchAndLock")
                    .Respond("application/json", @"[
                        {
                            ""id"": ""testTask"",
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
                Assert.Single(externalTasks);
                var firstTask = externalTasks.First();
                Assert.Equal("testTask", firstTask.Id);
                var testVariable = Assert.Contains("TEST", firstTask.Variables);
                Assert.Equal("testString", testVariable.Value);
                Assert.Equal(VariableType.String, testVariable.Type);
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
