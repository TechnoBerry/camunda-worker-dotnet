using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camunda.Worker.Client
{
    public class ExternalTaskClient : IExternalTaskClient
    {
        private readonly HttpClient _httpClient;

        public ExternalTaskClient(HttpClient httpClient)
        {
            _httpClient = Guard.NotNull(httpClient, nameof(httpClient));
            ValidateHttpClient(httpClient);
        }

        [ExcludeFromCodeCoverage]
        private static void ValidateHttpClient(HttpClient httpClient)
        {
            if (httpClient.BaseAddress == null)
            {
                throw new ArgumentException("BaseAddress must be configured", nameof(httpClient));
            }
        }

        public async Task<List<ExternalTask>> FetchAndLockAsync(
            FetchAndLockRequest request,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(request, nameof(request));

            using var response = await SendRequestAsync("/fetchAndLock", request, cancellationToken);
            await EnsureSuccessAsync(response);

            var externalTasks = await response.ReadJsonAsync<List<ExternalTask>>();
            return externalTasks;
        }

        public async Task CompleteAsync(
            string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using var response = await SendRequestAsync($"/{taskId}/complete", request, cancellationToken);
            await EnsureSuccessAsync(response);
        }

        public async Task ReportFailureAsync(
            string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using var response = await SendRequestAsync($"/{taskId}/failure", request, cancellationToken);
            await EnsureSuccessAsync(response);
        }

        public async Task ReportBpmnErrorAsync(
            string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using var response = await SendRequestAsync($"/{taskId}/bpmnError", request, cancellationToken);
            await EnsureSuccessAsync(response);
        }

        public async Task ExtendLockAsync(
            string taskId, ExtendLockRequest request,
            CancellationToken cancellationToken = default
        )
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using var response = await SendRequestAsync($"/{taskId}/extendLock", request, cancellationToken);
            await EnsureSuccessAsync(response);
        }

        private async Task<HttpResponseMessage> SendRequestAsync<T>(
            string path, T body, CancellationToken cancellationToken
        ) where T : notnull
        {
            var basePath = _httpClient.BaseAddress.AbsolutePath.TrimEnd('/');
            var requestPath = $"{basePath}/external-task/{path.TrimStart('/')}";
            var response = await _httpClient.PostJsonAsync(requestPath, body, cancellationToken);
            return response;
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode && response.IsJson())
            {
                var errorResponse = await response.ReadJsonAsync<ErrorResponse>();
                response.Content.Dispose();
                throw new ClientException(errorResponse, response.StatusCode);
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
