// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Api;
using Camunda.Worker.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Camunda.Worker
{
    public class DefaultCamundaWorker : ICamundaWorker
    {
        private readonly ICamundaApiClient _camundaApiClient;
        private readonly IExternalTaskHandler _handler;
        private readonly CamundaWorkerOptions _options;
        private readonly IReadOnlyList<FetchAndLockRequest.Topic> _topics;
        private readonly ILogger<DefaultCamundaWorker> _logger;

        public DefaultCamundaWorker(ICamundaApiClient camundaApiClient,
            IExternalTaskHandler handler,
            IOptions<CamundaWorkerOptions> options,
            IEnumerable<HandlerDescriptor> handlerDescriptors,
            ILogger<DefaultCamundaWorker> logger)
        {
            _camundaApiClient = camundaApiClient;
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
                    LockDuration = 60_000
                });
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var externalTask = await SelectExternalTask(cancellationToken);

                if (externalTask == null) continue;

                var result = await _handler.Process(externalTask, cancellationToken);

                await SendExecutionResult(externalTask.Id, result, cancellationToken);
            }
        }

        private async Task<ExternalTask> SelectExternalTask(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Waiting for external task");
            var externalTasks = await _camundaApiClient.FetchAndLock(new FetchAndLockRequest
            {
                WorkerId = _options.WorkerId,
                MaxTasks = 1,
                UsePriority = true,
                AsyncResponseTimeout = 10_000,
                Topics = _topics
            }, cancellationToken);

            return externalTasks.FirstOrDefault();
        }

        private async Task SendExecutionResult(string taskId, IExecutionResult result,
            CancellationToken cancellationToken)
        {
            switch (result)
            {
                case CompleteResult completeResult:
                    await _camundaApiClient.Complete(taskId, new CompleteRequest
                    {
                        WorkerId = _options.WorkerId,
                        Variables = completeResult.Variables
                    }, cancellationToken);
                    break;

                case FailureResult failureResult:
                    await _camundaApiClient.ReportFailure(taskId, new ReportFailureRequest
                    {
                        WorkerId = _options.WorkerId,
                        ErrorMessage = failureResult.ErrorMessage,
                        ErrorDetails = failureResult.ErrorDetails
                    }, cancellationToken);
                    break;
            }
        }
    }
}
