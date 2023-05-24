using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Camunda.Worker.Client;

internal static class HttpClientExtensions
{
    internal static bool IsJson(this HttpContentHeaders headers) =>
        headers.ContentType?.MediaType == MediaTypeNames.Application.Json;

    internal static bool IsJson(this HttpResponseMessage message) =>
        message.Content.Headers.IsJson();
}
