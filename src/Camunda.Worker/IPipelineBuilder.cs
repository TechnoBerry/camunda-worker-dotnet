using System;

namespace Camunda.Worker;

public interface IPipelineBuilder
{
    IServiceProvider ApplicationServices { get; }

    IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware);

    ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate);
}
