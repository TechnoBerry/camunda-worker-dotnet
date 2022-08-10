using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Routing;

internal static class ExternalTaskRouter
{
    internal static async Task RouteAsync(IExternalTaskContext context)
    {
        Guard.NotNull(context, nameof(context));
        var provider = context.ServiceProvider;

        var endpointProvider = provider.GetRequiredService<IEndpointProvider>();
        var endpoint = endpointProvider.GetEndpoint(context.Task);

        if (endpoint is null)
        {
            throw new ApplicationException($"Endpoint for external task {context.Task.Id} could not be resolved");
        }

        await endpoint.HandlerDelegate(context);
    }
}
