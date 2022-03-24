using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddExternalTaskClient(this IServiceCollection services)
    {
        return services.AddHttpClient<IExternalTaskClient, ExternalTaskClient>();
    }

    public static IHttpClientBuilder AddExternalTaskClient(this IServiceCollection services, Action<HttpClient> configureClient)
    {
        return services.AddHttpClient<IExternalTaskClient, ExternalTaskClient>(configureClient);
    }
}
