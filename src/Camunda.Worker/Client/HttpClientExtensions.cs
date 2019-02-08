// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Camunda.Worker.Client
{
    internal static class HttpClientExtensions
    {
        internal static async Task<T> ReadAsObjectAsync<T>(this HttpContent content,
            JsonSerializerSettings serializerSettings)
        {
            var jsonResponse = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(jsonResponse, serializerSettings);
            return result;
        }
    }
}
