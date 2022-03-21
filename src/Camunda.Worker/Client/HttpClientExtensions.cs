using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Camunda.Worker.Client;

internal static class HttpClientExtensions
{
    private static readonly JsonSerializerSettings SerializerSettings = MakeSerializerSettings();
    private static readonly JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);

    private static JsonSerializerSettings MakeSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false,
                    OverrideSpecifiedNames = true
                }
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        settings.Converters.Add(new StringEnumConverter());

        return settings;
    }

    internal static async Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient client,
        string path,
        T requestBody,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        var requestContent = JsonContent.Create(requestBody, Serializer);
        var response = await client.PostAsync(path, requestContent, cancellationToken);
        return response;
    }

    internal static Task<T> ReadJsonAsync<T>(this HttpResponseMessage responseMessage) =>
        responseMessage.Content.ReadJsonAsync<T>();

    internal static async Task<T> ReadJsonAsync<T>(this HttpContent content)
    {
        using var stream = await content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);

        var result = Serializer.Deserialize<T>(jsonReader);
        return result;
    }

    internal static bool IsJson(this HttpContentHeaders headers) =>
        headers.ContentType.MediaType == JsonContent.JsonContentType;

    internal static bool IsJson(this HttpResponseMessage message) =>
        message.Content?.Headers?.IsJson() ?? false;
}
