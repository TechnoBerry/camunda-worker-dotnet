// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Camunda.Worker.Client
{
    public class ExternalTaskCamundaClientTest
    {
        private readonly Mock<FakeMessageHandler> _handlerMock = new Mock<FakeMessageHandler> {CallBase = true};

        [Fact]
        public async Task TestFetchAndLock()
        {
            using (var client = MakeClient())
            {
                HttpRequestMessage httpRequest = null;

                _handlerMock.Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
                    .Callback((HttpRequestMessage req) => httpRequest = req)
                    .Returns(() => new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(@"[
                            {
                                ""id"": ""testTask"",
                                ""variables"": {
                                    ""TEST"": {
                                        ""value"": ""testString"",
                                        ""type"": ""String""
                                    }
                                }
                            }
                        ]", Encoding.UTF8, "application/json")
                    });

                var request = new FetchAndLockRequest("testWorker", 10)
                {
                    AsyncResponseTimeout = 10_000,
                    Topics = new[]
                    {
                        new FetchAndLockRequest.Topic("testTopic", 10_000)
                    }
                };

                var externalTasks = await client.FetchAndLock(request, CancellationToken.None);

                Assert.NotNull(httpRequest);
                Assert.Equal(new Uri("http://test/api/external-task/fetchAndLock"), httpRequest.RequestUri);

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
                HttpRequestMessage httpRequest = null;

                _handlerMock.Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
                    .Callback((HttpRequestMessage req) => httpRequest = req)
                    .Returns(() => new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        Content = new StringContent("")
                    });

                var request = new CompleteRequest("testWorker")
                {
                    Variables = new Dictionary<string, Variable>
                    {
                        ["TEST"] = new Variable("testString")
                    }
                };

                await client.Complete("testTask", request, CancellationToken.None);

                Assert.NotNull(httpRequest);
                Assert.Equal(new Uri("http://test/api/external-task/testTask/complete"), httpRequest.RequestUri);
                var requestContent = await httpRequest.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestContent);
                Assert.True(requestObject.ContainsKey("variables"));
            }
        }

        [Fact]
        public async Task TestReportFailure()
        {
            using (var client = MakeClient())
            {
                HttpRequestMessage httpRequest = null;

                _handlerMock.Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
                    .Callback((HttpRequestMessage req) => httpRequest = req)
                    .Returns(() => new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        Content = new StringContent("")
                    });

                var request = new ReportFailureRequest("testWorker")
                {
                    ErrorMessage = "Error",
                    ErrorDetails = "Details"
                };

                await client.ReportFailure("testTask", request, CancellationToken.None);

                Assert.NotNull(httpRequest);
                Assert.Equal(new Uri("http://test/api/external-task/testTask/failure"), httpRequest.RequestUri);
            }
        }

        [Fact]
        public async Task TestReportBpmnError()
        {
            using (var client = MakeClient())
            {
                HttpRequestMessage httpRequest = null;

                _handlerMock.Setup(handler => handler.Send(It.IsAny<HttpRequestMessage>()))
                    .Callback((HttpRequestMessage req) => httpRequest = req)
                    .Returns(() => new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NoContent,
                        Content = new StringContent("")
                    });

                var request = new BpmnErrorRequest("testWorker", "testCode")
                {
                    ErrorMessage = "Error",
                    Variables = new Dictionary<string, Variable>()
                };

                await client.ReportBpmnError("testTask", request, CancellationToken.None);

                Assert.NotNull(httpRequest);
                Assert.Equal(new Uri("http://test/api/external-task/testTask/bpmnError"), httpRequest.RequestUri);
            }
        }

        private ExternalTaskCamundaClient MakeClient()
        {
            return new ExternalTaskCamundaClient(
                new HttpClient(_handlerMock.Object)
                {
                    BaseAddress = new Uri("http://test/api")
                }
            );
        }


        public class FakeMessageHandler : HttpMessageHandler
        {
            public virtual HttpResponseMessage Send(HttpRequestMessage request)
            {
                throw new NotImplementedException();
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(Send(request));
            }
        }
    }
}
