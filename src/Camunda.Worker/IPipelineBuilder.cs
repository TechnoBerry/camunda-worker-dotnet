using System;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface IPipelineBuilder
    {
        IServiceCollection Services { get; }

        IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware);

        [Obsolete]
        ExternalTaskDelegate Build();

        ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate);
    }
}
