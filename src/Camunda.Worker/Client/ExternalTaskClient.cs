using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client.Serialization;

namespace Camunda.Worker.Client;

public class ExternalTaskClient : IExternalTaskClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ExternalTaskClient(HttpClient httpClient)
    {
        _httpClient = Guard.NotNull(httpClient, nameof(httpClient));
        ValidateHttpClient(httpClient);

        _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
            .Also(opts =>
            {
                opts.Converters.Add(new JsonStringEnumConverter());
                opts.Converters.Add(new VariableJsonConverter());
                opts.Converters.Add(new JsonVariableJsonConverter());
                opts.Converters.Add(new XmlVariableJsonConverter());
            });
    }

    public ExternalTaskClient(HttpClient httpClient, Action<JsonSerializerOptions> configureJsonOptions)
        : this(httpClient)
    {
        _jsonSerializerOptions = _jsonSerializerOptions
            .Also(configureJsonOptions);
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

        var externalTasks = await response.Content
            .ReadFromJsonAsync<List<ExternalTask>>(_jsonSerializerOptions, cancellationToken);

        return externalTasks ?? new List<ExternalTask>();
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
        var basePath = _httpClient.BaseAddress?.AbsolutePath.TrimEnd('/') ?? string.Empty;
        var requestPath = $"{basePath}/external-task/{path.TrimStart('/')}";
        var response = await _httpClient.PostAsJsonAsync(requestPath, body, _jsonSerializerOptions, cancellationToken);
        return response;
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode && response.IsJson())
        {
            var errorResponse = await response.Content
                .ReadFromJsonAsync<ErrorResponse>(_jsonSerializerOptions);
            response.Content.Dispose();
            throw new ClientException(errorResponse ?? new ErrorResponse(), response.StatusCode);
        }

        response.EnsureSuccessStatusCode();
    }
}
