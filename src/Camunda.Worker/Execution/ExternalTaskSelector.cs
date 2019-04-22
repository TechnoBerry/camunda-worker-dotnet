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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskSelector : IExternalTaskSelector
    {
        private readonly IExternalTaskClient _client;
        private readonly CamundaWorkerOptions _options;
        private readonly ILogger<ExternalTaskSelector> _logger;

        public ExternalTaskSelector(IExternalTaskClient client, IOptions<CamundaWorkerOptions> options,
            ILogger<ExternalTaskSelector> logger = default)
        {
            _client = Guard.NotNull(client, nameof(client));
            _options = Guard.NotNull(options, nameof(options)).Value;
            _logger = logger ?? new NullLogger<ExternalTaskSelector>();
        }

        public async Task<IEnumerable<ExternalTask>> SelectAsync(IEnumerable<FetchAndLockRequest.Topic> topics,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Waiting for external task");

                var fetchAndLockRequest = GetRequest(topics, _options);

                var externalTasks = await _client.FetchAndLock(fetchAndLockRequest, cancellationToken);

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

        protected virtual FetchAndLockRequest GetRequest(IEnumerable<FetchAndLockRequest.Topic> topics,
            CamundaWorkerOptions options)
        {
            var fetchAndLockRequest = new FetchAndLockRequest(options.WorkerId)
            {
                UsePriority = true,
                AsyncResponseTimeout = options.AsyncResponseTimeout,
                Topics = topics
            };

            return fetchAndLockRequest;
        }
    }
}
