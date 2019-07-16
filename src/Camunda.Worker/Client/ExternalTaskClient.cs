using System.Collections.Generic;
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
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest("fetchAndLock", request, cancellationToken))
            {
                await EnsureSuccess(response);

                var externalTasks = await response.ReadJsonAsync<IList<ExternalTask>>();
                return externalTasks;
            }
        }

        public async Task Complete(string taskId, CompleteRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest($"{taskId}/complete", request, cancellationToken))
            {
                await EnsureSuccess(response);
            }
        }

        public async Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest($"{taskId}/failure", request, cancellationToken))
            {
                await EnsureSuccess(response);
            }
        }

        public async Task ReportBpmnError(string taskId, BpmnErrorRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest($"{taskId}/bpmnError", request, cancellationToken))
            {
                await EnsureSuccess(response);
            }
        }

        public async Task ExtendLock(string taskId, ExtendLockRequest request,
            CancellationToken cancellationToken = default)
        {
            Guard.NotNull(taskId, nameof(taskId));
            Guard.NotNull(request, nameof(request));

            using (var response = await SendRequest($"{taskId}/extendLock", request, cancellationToken))
            {
                await EnsureSuccess(response);
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string path, object body,
            CancellationToken cancellationToken)
        {
            var basePath = _httpClient.BaseAddress.AbsolutePath.TrimEnd('/');
            var requestPath = $"{basePath}/external-task/{path}";
            var response = await _httpClient.PostJsonAsync(requestPath, body, cancellationToken);
            return response;
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
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
