using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IEndpointProvider _endpointProvider;
        private readonly ILogger<ExternalTaskRouter> _logger;

        public ExternalTaskRouter(IEndpointProvider endpointProvider,
            ILogger<ExternalTaskRouter> logger = null)
        {
            _endpointProvider = Guard.NotNull(endpointProvider, nameof(endpointProvider));
            _logger = logger ?? NullLogger<ExternalTaskRouter>.Instance;
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var externalTaskDelegate = _endpointProvider.GetEndpointDelegate(context.Task);
            var externalTask = context.Task;

            _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);

            await externalTaskDelegate(context);

            _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);
        }
    }
}
