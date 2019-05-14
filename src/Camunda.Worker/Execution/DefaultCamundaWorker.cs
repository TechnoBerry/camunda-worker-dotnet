using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskRouter _router;
        private readonly ITopicsProvider _topicsProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IExternalTaskSelector _selector;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(IExternalTaskRouter router,
            ITopicsProvider topicsProvider,
            IExternalTaskSelector selector,
            IServiceProvider serviceProvider,
            ILogger<DefaultCamundaWorker> logger = default)
        {
            _router = Guard.NotNull(router, nameof(router));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _selector = Guard.NotNull(selector, nameof(selector));
            _serviceProvider = Guard.NotNull(serviceProvider, nameof(serviceProvider));
            _logger = logger ?? new NullLogger<DefaultCamundaWorker>();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTasks = await SelectExternalTasks(cancellationToken);

                var activeAsyncTasks = externalTasks
                    .Select(CreateContext)
                    .Select(ExecuteInContext)
                    .ToList();

                await Task.WhenAll(activeAsyncTasks);
            }
        }

        private Task<IEnumerable<ExternalTask>> SelectExternalTasks(CancellationToken cancellationToken)
        {
            var topics = _topicsProvider.GetTopics();
            var selectedTasks = _selector.SelectAsync(topics, cancellationToken);
            return selectedTasks;
        }

        private ExternalTaskContext CreateContext(ExternalTask externalTask)
        {
            var scope = _serviceProvider.CreateScope();
            var context = new ExternalTaskContext(externalTask, scope);
            return context;
        }

        private async Task ExecuteInContext(IExternalTaskContext context)
        {
            using (context)
            {
                try
                {
                    await _router.RouteAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed execution of task {Id}", context.Task.Id);
                }
            }
        }
    }
}
