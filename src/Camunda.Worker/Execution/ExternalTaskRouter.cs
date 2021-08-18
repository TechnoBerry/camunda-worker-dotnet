using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));
            var provider = context.ServiceProvider;

            var endpointProvider = provider.GetRequiredService<IEndpointProvider>();
            var externalTaskDelegate = endpointProvider.GetEndpointDelegate(context.Task);
            await externalTaskDelegate(context);
        }
    }
}
