using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Camunda.Worker.Client;

internal sealed class JsonContent : HttpContent
{
    internal const string JsonContentType = "application/json";

    private readonly object _body;
    private readonly Type _bodyType;
    private readonly JsonSerializer _serializer;

    private JsonContent(object body, Type bodyType, JsonSerializer serializer)
    {
        _body = body;
        _bodyType = bodyType;
        _serializer = serializer;

        Headers.ContentType = new MediaTypeHeaderValue(JsonContentType)
        {
            CharSet = Encoding.UTF8.WebName
        };
    }

    internal static JsonContent Create<T>(T body, JsonSerializer serializer) where T : notnull
    {
        return new JsonContent(body, typeof(T), serializer);
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var streamWriter = new StreamWriter(stream);
        _serializer.Serialize(streamWriter, _body, _bodyType);
        await streamWriter.FlushAsync();
    }

    protected override bool TryComputeLength(out long length)
    {
        length = default;
        return false;
    }
}
