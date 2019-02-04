// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskCamundaClient _externalTaskCamundaClient;
        private readonly IGeneralExternalTaskHandler _handler;
        private readonly CamundaWorkerOptions _options;
        private readonly ITopicsProvider _topicsProvider;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(IExternalTaskCamundaClient externalTaskCamundaClient,
            IGeneralExternalTaskHandler handler,
            IOptions<CamundaWorkerOptions> options,
            ITopicsProvider topicsProvider,
            ILogger<DefaultCamundaWorker> logger = null)
        {
            _externalTaskCamundaClient = externalTaskCamundaClient;
            _handler = handler;
            _options = options.Value;
            _topicsProvider = topicsProvider;
            _logger = logger ?? new NullLogger<DefaultCamundaWorker>();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTask = await SelectExternalTask(cancellationToken);

                if (externalTask == null) continue;

                var context = new ExternalTaskContext(externalTask, _externalTaskCamundaClient);

                var result = await _handler.Process(externalTask);

                await result.ExecuteResult(context);
            }
        }

        private async Task<ExternalTask> SelectExternalTask(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Waiting for external task");
                var externalTasks = await _externalTaskCamundaClient.FetchAndLock(new FetchAndLockRequest
                {
                    WorkerId = _options.WorkerId,
                    MaxTasks = 1,
                    UsePriority = true,
                    AsyncResponseTimeout = _options.AsyncResponseTimeout,
                    Topics = _topicsProvider.GetTopics()
                }, cancellationToken);

                return externalTasks.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError("Failed receiving of external tasks. Reason: \"{Reason}\"", e.Message);
                await Wait(10_000, cancellationToken);
                return null;
            }
        }

        private static async Task Wait(int seconds, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(seconds, cancellationToken);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
