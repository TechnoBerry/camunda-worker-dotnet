using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Camunda.Worker.Client
{
    internal static class HttpClientExtensions
    {
        private const string JsonContentType = "application/json";
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

        internal static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client,
            string path,
            object requestBody,
            CancellationToken cancellationToken = default)
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                Serializer.Serialize(jsonWriter, requestBody);
                await jsonWriter.FlushAsync(cancellationToken);

                stream.Position = 0;

                var requestContent = new StreamContent(stream);
                requestContent.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType)
                {
                    CharSet = Encoding.UTF8.WebName
                };

                var response = await client.PostAsync(path, requestContent, cancellationToken);
                return response;
            }
        }

        internal static async Task<T> ReadAsObjectAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var result = Serializer.Deserialize<T>(jsonReader);
                return result;
            }
        }

        internal static bool IsJson(this HttpContentHeaders headers) =>
            headers.ContentType.MediaType == JsonContentType;
    }
}
