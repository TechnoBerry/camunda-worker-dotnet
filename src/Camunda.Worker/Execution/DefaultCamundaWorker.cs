#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskCamundaClient _externalTaskCamundaClient;
        private readonly IExternalTaskRouter _router;
        private readonly ITopicsProvider _topicsProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CamundaWorkerOptions _options;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(IExternalTaskCamundaClient externalTaskCamundaClient,
            IExternalTaskRouter router,
            ITopicsProvider topicsProvider,
            IServiceScopeFactory scopeFactory,
            IOptions<CamundaWorkerOptions> options,
            ILogger<DefaultCamundaWorker> logger = null)
        {
            _externalTaskCamundaClient = Guard.NotNull(externalTaskCamundaClient, nameof(externalTaskCamundaClient));
            _router = Guard.NotNull(router, nameof(router));
            _topicsProvider = Guard.NotNull(topicsProvider, nameof(topicsProvider));
            _scopeFactory = Guard.NotNull(scopeFactory, nameof(scopeFactory));
            _options = Guard.NotNull(options, nameof(options)).Value;
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

        private async Task<IEnumerable<ExternalTask>> SelectExternalTasks(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Waiting for external task");

                var fetchAndLockRequest = new FetchAndLockRequest(_options.WorkerId)
                {
                    UsePriority = true,
                    AsyncResponseTimeout = _options.AsyncResponseTimeout,
                    Topics = _topicsProvider.GetTopics()
                };

                var externalTasks = await _externalTaskCamundaClient.FetchAndLock(
                    fetchAndLockRequest, cancellationToken
                );

                _logger.LogDebug("Locked {Count} external tasks", externalTasks.Count);

                return externalTasks;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await Task.Delay(10_000, cancellationToken);
                return Enumerable.Empty<ExternalTask>();
            }
        }

        private ExternalTaskContext CreateContext(ExternalTask externalTask)
        {
            var scope = _scopeFactory.CreateScope();
            var context = new ExternalTaskContext(externalTask, scope);
            return context;
        }

        private async Task ExecuteInContext(ExternalTaskContext context)
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
