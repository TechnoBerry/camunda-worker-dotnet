using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddExternalTaskClient(this IServiceCollection services)
    {
        return services.AddHttpClient<IExternalTaskClient>()
            .AddTypedClient<IExternalTaskClient>(httpClient => new ExternalTaskClient(httpClient));
    }

    public static IHttpClientBuilder AddExternalTaskClient(this IServiceCollection services, Action<HttpClient> configureClient)
    {
        return services.AddHttpClient<IExternalTaskClient>(configureClient)
            .AddTypedClient<IExternalTaskClient>(httpClient => new ExternalTaskClient(httpClient));
    }

    public static IHttpClientBuilder AddExternalTaskClient(this IServiceCollection services, Action<HttpClient> configureClient, Action<JsonSerializerOptions> configureJsonOptions)
    {
        return services.AddHttpClient<IExternalTaskClient>(configureClient)
            .AddTypedClient<IExternalTaskClient>(httpClient => new ExternalTaskClient(httpClient, configureJsonOptions));
    }
}
