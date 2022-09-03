using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Camunda.Worker;

public class CamundaWorkerBuilderTest
{
    private readonly ServiceCollection _services = new();
    private readonly CamundaWorkerBuilder _builder;

    public CamundaWorkerBuilderTest()
    {
        _builder = new CamundaWorkerBuilder(_services, "testWorker");
    }

    [Fact]
    public void TestAddHandler()
    {
        Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

        _builder.AddHandler(FakeHandlerDelegate, new HandlerMetadata(new[] {"testTopic"}));

        Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                        d.ImplementationInstance != null);
    }

    [Fact]
    public void TestAddEndpointResolver()
    {
        _builder.AddEndpointResolver((_, _) => new EndpointResolver());

        using var provider = _services.BuildServiceProvider();

        Assert.IsType<EndpointResolver>(provider.GetService<IEndpointResolver>());
    }

    [Fact]
    public void TestAddFetchAndLockRequestProvider()
    {
        _builder.AddFetchAndLockRequestProvider((_, _) => new FetchAndLockRequestProvider());

        using var provider = _services.BuildServiceProvider();

        Assert.IsType<FetchAndLockRequestProvider>(provider.GetService<IFetchAndLockRequestProvider>());
    }

    [Fact]
    public void TestConfigurePipeline()
    {
        _builder.ConfigurePipeline(pipeline => { });

        Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                        d.ServiceType == typeof(WorkerHandlerDescriptor));
    }

    [Fact]
    public void TestConfigureEvents()
    {
        _builder.ConfigureEvents(events =>
        {
            events.OnBeforeFetchAndLock = (_, _) => Task.CompletedTask;
        });

        Assert.Contains(_services, d => d.ServiceType == typeof(IConfigureOptions<WorkerEvents>));
    }

    private class EndpointResolver : IEndpointResolver
    {

        public Endpoint? Resolve(ExternalTask externalTask)
        {
            throw new NotImplementedException();
        }
    }

    private class FetchAndLockRequestProvider : IFetchAndLockRequestProvider
    {
        public FetchAndLockRequest GetRequest()
        {
            throw new NotImplementedException();
        }
    }
}
