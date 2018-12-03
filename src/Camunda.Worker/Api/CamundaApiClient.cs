// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Camunda.Worker.Api
{
    public class CamundaApiClient : ICamundaApiClient, IDisposable
    {
        private const string JsonContentType = "application/json";
        private static readonly JsonSerializerSettings SerializerSettings = MakeSerializerSettings();

        private readonly HttpClient _httpClient;

        public CamundaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<IList<ExternalTask>> FetchAndLock(FetchAndLockRequest request,
            CancellationToken cancellationToken)
        {
            using (var response = await SendRequest("external-task/fetchAndLock", request, cancellationToken))
            {
                return await ParseResponseContent<IList<ExternalTask>>(response.Content);
            }
        }

        public async Task Complete(string taskId, CompleteRequest request, CancellationToken cancellationToken)
        {
            using (await SendRequest($"external-task/{taskId}/complete", request, cancellationToken))
            {
            }
        }

        public async Task ReportFailure(string taskId, ReportFailureRequest request,
            CancellationToken cancellationToken)
        {
            using (await SendRequest($"external-task/{taskId}/failure", request, cancellationToken))
            {
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string path, object requestBody,
            CancellationToken cancellationToken)
        {
            var basePath = _httpClient.BaseAddress.AbsolutePath.TrimEnd('/');
            var requestContent = MakeRequestContent(requestBody);
            var response = await _httpClient.PostAsync($"{basePath}/{path}", requestContent, cancellationToken);
            return response;
        }

        private static HttpContent MakeRequestContent(object requestBody)
        {
            var jsonRequestBody = JsonConvert.SerializeObject(requestBody, SerializerSettings);
            return new StringContent(jsonRequestBody, Encoding.UTF8, JsonContentType);
        }

        private static async Task<T> ParseResponseContent<T>(HttpContent content)
        {
            var jsonResponse = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse, SerializerSettings);
        }

        private static JsonSerializerSettings MakeSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            };
        }
    }
}
