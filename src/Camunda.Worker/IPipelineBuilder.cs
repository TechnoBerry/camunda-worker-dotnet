using System;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker
{
    public interface IPipelineBuilder
    {
        IServiceCollection Services { get; }

        IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware);

        ExternalTaskDelegate Build();
    }
}
