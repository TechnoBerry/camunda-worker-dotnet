using System;

namespace Camunda.Worker;

public delegate TService WorkerServiceFactory<out TService>(WorkerIdString workerId, IServiceProvider serviceProvider);
