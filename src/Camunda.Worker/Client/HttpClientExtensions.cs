// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Camunda.Worker.Client
{
    internal static class HttpClientExtensions
    {
        private const string JsonContentType = "application/json";

        internal static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client,
            string path,
            object requestBody,
            JsonSerializerSettings serializerSettings,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var jsonRequestBody = JsonConvert.SerializeObject(requestBody, serializerSettings);
            var requestContent = new StringContent(jsonRequestBody, Encoding.UTF8, JsonContentType);
            var response = await client.PostAsync(path, requestContent, cancellationToken);
            return response;
        }

        internal static async Task<T> ReadAsObjectAsync<T>(this HttpContent content,
            JsonSerializerSettings serializerSettings)
        {
            var jsonResponse = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(jsonResponse, serializerSettings);
            return result;
        }
    }
}
