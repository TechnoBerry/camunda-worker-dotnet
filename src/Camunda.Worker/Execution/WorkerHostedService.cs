#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Camunda.Worker.Execution
{
    public sealed class WorkerHostedService : BackgroundService
    {
        private readonly ICamundaWorker _worker;
        private readonly CamundaWorkerOptions _options;

        public WorkerHostedService(ICamundaWorker worker, IOptions<CamundaWorkerOptions> options)
        {
            _worker = Guard.NotNull(worker, nameof(worker));
            _options = Guard.NotNull(options, nameof(options)).Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var activeTasks = Enumerable.Range(0, _options.WorkerCount)
                .Select(_ => _worker.Run(stoppingToken));
            return Task.WhenAll(activeTasks);
        }
    }
}
