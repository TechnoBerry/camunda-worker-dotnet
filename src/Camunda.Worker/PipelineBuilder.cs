using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker;

public class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Func<ExternalTaskDelegate, ExternalTaskDelegate>> _middlewareList = new();

    public PipelineBuilder(string workerId, IServiceProvider serviceProvider)
    {
        WorkerId = workerId;
        ApplicationServices = serviceProvider;
    }

    public string WorkerId { get; }

    public IServiceProvider ApplicationServices { get; }

    public IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware)
    {
        _middlewareList.Add(middleware);
        return this;
    }

    public ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate)
    {
        Guard.NotNull(lastDelegate, nameof(lastDelegate));

        return _middlewareList.AsEnumerable()
            .Reverse()
            .Aggregate(lastDelegate, (current, middleware) => middleware(current));
    }
}
