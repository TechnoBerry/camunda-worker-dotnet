using System;
using System.Collections.Generic;

namespace Camunda.Worker;

public class PipelineBuilder : IPipelineBuilder
{
    private readonly List<Func<ExternalTaskDelegate, ExternalTaskDelegate>> _middlewareList = new();

    public PipelineBuilder(IServiceProvider serviceProvider, WorkerIdString workerId)
    {
        ApplicationServices = serviceProvider;
        WorkerId = workerId;
    }

    public IServiceProvider ApplicationServices { get; }

    public WorkerIdString WorkerId { get; }

    public IPipelineBuilder Use(Func<ExternalTaskDelegate, ExternalTaskDelegate> middleware)
    {
        _middlewareList.Add(middleware);
        return this;
    }

    public ExternalTaskDelegate Build(ExternalTaskDelegate lastDelegate)
    {
        var result = lastDelegate;

        for (var i = _middlewareList.Count - 1; i >= 0 ; i--)
        {
            result = _middlewareList[i](result);
        }

        return result;
    }
}
