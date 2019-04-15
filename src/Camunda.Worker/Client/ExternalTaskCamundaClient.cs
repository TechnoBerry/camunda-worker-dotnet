#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public class ExternalTaskCamundaClient : IExternalTaskCamundaClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        public ExternalTaskCamundaClient(HttpClient httpClient)
        {
            _httpClient = Guard.NotNull(httpClient, nameof(httpClient));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest("external-task/fetchAndLock", request, cancellationToken))
            {
                var externalTasks = await response.Content.ReadAsObjectAsync<IList<ExternalTask>>();
                return externalTasks;
            }
        }

        public async Task Complete(string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (await SendRequest($"external-task/{taskId}/complete", request, cancellationToken))
            {
            }
        }

        public async Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (await SendRequest($"external-task/{taskId}/failure", request, cancellationToken))
            {
            }
        }

        public async Task ReportBpmnError(string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (await SendRequest($"external-task/{taskId}/bpmnError", request, cancellationToken))
            {
            }
        }

        public async Task ExtendLock(string taskId, ExtendLockRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (await SendRequest($"external-task/{taskId}/extendLock", request, cancellationToken))
            {
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string path, object body,
            CancellationToken cancellationToken)
        {
            var basePath = _httpClient.BaseAddress.AbsolutePath.TrimEnd('/');
            var requestPath = $"{basePath}/{path}";
            var response = await _httpClient.PostJsonAsync(requestPath, body, cancellationToken);

            if (!response.IsSuccessStatusCode && response.Content != null)
            {
                var errorResponse = await response.Content.ReadAsObjectAsync<ErrorResponse>();
                response.Content.Dispose();
                throw new ClientException(errorResponse, response.StatusCode);
            }

            return response.EnsureSuccessStatusCode();
        }
    }
}
