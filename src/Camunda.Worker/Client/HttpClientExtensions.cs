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
            var jsonRequestBody = JsonConvert.SerializeObject(requestBody, SerializerSettings);
            var requestContent = new StringContent(jsonRequestBody, Encoding.UTF8, JsonContentType);
            var response = await client.PostAsync(path, requestContent, cancellationToken);
            return response;
        }

        internal static async Task<T> ReadAsObjectAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = JsonSerializer.Create(SerializerSettings);
                var result = serializer.Deserialize<T>(jsonReader);
                return result;
            }
        }

        internal static bool IsJson(this HttpContentHeaders headers) =>
            headers.ContentType.MediaType == JsonContentType;
    }
}
