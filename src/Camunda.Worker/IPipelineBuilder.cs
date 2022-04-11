using System;

namespace Camunda.Worker;

public interface IPipelineBuilder
{
    string WorkerId { get; }

    IServiceProvider ApplicationServices { get; }

    IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware);

    ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate);
}
