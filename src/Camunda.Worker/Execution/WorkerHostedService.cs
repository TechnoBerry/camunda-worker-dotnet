// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Camunda.Worker.Execution
{
    public sealed class WorkerHostedService : BackgroundService
    {
        private readonly ICamundaWorker _worker;

        public WorkerHostedService(ICamundaWorker worker)
        {
            _worker = Guard.NotNull(worker, nameof(worker));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => _worker.Run(stoppingToken);
    }
}
