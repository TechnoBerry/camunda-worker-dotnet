using System;

namespace Camunda.Worker;

public interface IPipelineBuilder
{
    IServiceProvider ApplicationServices { get; }

    WorkerIdString WorkerId { get; }

    IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware);

    ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate);
}
