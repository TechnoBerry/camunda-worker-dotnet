using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IHandlerDelegateProvider _handlerDelegateProvider;
        private readonly ILogger<ExternalTaskRouter> _logger;

        public ExternalTaskRouter(IHandlerDelegateProvider handlerDelegateProvider,
            ILogger<ExternalTaskRouter> logger = default)
        {
            _handlerDelegateProvider = Guard.NotNull(handlerDelegateProvider, nameof(handlerDelegateProvider));
            _logger = logger ?? new NullLogger<ExternalTaskRouter>();
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var handler = MakeHandler(context);
            var externalTask = context.Task;

            _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);

            await handler.HandleAsync(context);

            _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);
        }

        private IExternalTaskHandler MakeHandler(IExternalTaskContext context)
        {
            var externalTask = context.Task;
            var handlerFactory = _handlerDelegateProvider.GetHandlerFactory(externalTask);
            var handler = handlerFactory(context.ServiceProvider);
            return handler;
        }
    }
}
