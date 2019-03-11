// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Camunda.Worker.Client
{
    public class ExternalTaskCamundaClient : IExternalTaskCamundaClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        public ExternalTaskCamundaClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await SendRequest("external-task/fetchAndLock", request, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var externalTasks = await response.Content.ReadAsObjectAsync<IList<ExternalTask>>();
                return externalTasks;
            }
        }

        public async Task Complete(string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (await SendRequest($"external-task/{taskId}/complete", request, cancellationToken))
            {
            }
        }

        public async Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (await SendRequest($"external-task/{taskId}/failure", request, cancellationToken))
            {
            }
        }

        public async Task ReportBpmnError(string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (await SendRequest($"external-task/{taskId}/bpmnError", request, cancellationToken))
            {
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string path, object body,
            CancellationToken cancellationToken)
        {
            var basePath = _httpClient.BaseAddress.AbsolutePath.TrimEnd('/');
            var requestPath = $"{basePath}/{path}";
            var response = await _httpClient.PostJsonAsync(requestPath, body, cancellationToken);
            return response;
        }
    }
}
