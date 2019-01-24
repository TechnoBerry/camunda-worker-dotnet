// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Camunda.Worker.Execution
{
    public class WorkerHostedService : IHostedService
    {
        private readonly ICamundaWorker _worker;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private Task _activeTask;

        public WorkerHostedService(ICamundaWorker worker)
        {
            _worker = worker;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _activeTask = _worker.Run(_cts.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_activeTask == null)
            {
                return;
            }

            _cts.Cancel();

            await Task.WhenAny(_activeTask, Task.Delay(-1, cancellationToken));
        }
    }
}
