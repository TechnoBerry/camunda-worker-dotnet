using System.Threading.Tasks;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IEndpointProvider _endpointProvider;

        public ExternalTaskRouter(
            IEndpointProvider endpointProvider
        )
        {
            _endpointProvider = Guard.NotNull(endpointProvider, nameof(endpointProvider));
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var externalTaskDelegate = _endpointProvider.GetEndpointDelegate(context.Task);
            await externalTaskDelegate(context);
        }
    }
}
