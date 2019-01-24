// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Camunda.Worker
{
    public class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly IExternalTaskCamundaClient _externalTaskCamundaClient;
        private readonly IGeneralExternalTaskHandler _handler;
        private readonly CamundaWorkerOptions _options;
        private readonly IReadOnlyList<FetchAndLockRequest.Topic> _topics;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(IExternalTaskCamundaClient externalTaskCamundaClient,
            IGeneralExternalTaskHandler handler,
            IOptions<CamundaWorkerOptions> options,
            IEnumerable<HandlerDescriptor> handlerDescriptors,
            ILogger<DefaultCamundaWorker> logger)
        {
            _externalTaskCamundaClient = externalTaskCamundaClient;
            _handler = handler;
            _options = options.Value;
            _topics = ExtractTopics(handlerDescriptors).ToList();
            _logger = logger;
        }

        private static IEnumerable<FetchAndLockRequest.Topic> ExtractTopics(IEnumerable<HandlerDescriptor> descriptors)
        {
            return descriptors
                .Select(descriptor => new FetchAndLockRequest.Topic
                {
                    TopicName = descriptor.TopicName,
                    LockDuration = descriptor.LockDuration,
                    LocalVariables = descriptor.LocalVariables,
                    Variables = descriptor.Variables
                });
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTask = await SelectExternalTask(cancellationToken);

                if (externalTask == null) continue;

                var context = new ExternalTaskContext(externalTask, _externalTaskCamundaClient);

                var result = await _handler.Process(externalTask);

                await result.ExecuteResult(context, cancellationToken);
            }
        }

        private async Task<ExternalTask> SelectExternalTask(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Waiting for external task");
            var externalTasks = await _externalTaskCamundaClient.FetchAndLock(new FetchAndLockRequest
            {
                WorkerId = _options.WorkerId,
                MaxTasks = 1,
                UsePriority = true,
                AsyncResponseTimeout = 10_000,
                Topics = _topics
            }, cancellationToken);

            return externalTasks.FirstOrDefault();
        }
    }
}
