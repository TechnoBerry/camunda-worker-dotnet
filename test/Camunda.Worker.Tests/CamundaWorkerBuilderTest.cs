using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
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
        _builder = new CamundaWorkerBuilder(_services, new WorkerIdString("testWorker"));
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
    public void TestAddFactoryProvider()
    {
        _builder.AddEndpointProvider<EndpointProvider>();

        Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                        d.ServiceType == typeof(IEndpointProvider) &&
                                        d.ImplementationType == typeof(EndpointProvider));
    }

    [Fact]
    public void TestAddTopicsProvider()
    {
        _builder.AddTopicsProvider<TopicsProvider>();

        using var provider = _services.BuildServiceProvider();

        Assert.IsType<TopicsProvider>(provider.GetService<ITopicsProvider>());
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

    private class EndpointProvider : IEndpointProvider
    {
        public ExternalTaskDelegate GetEndpointDelegate(ExternalTask externalTask)
        {
            throw new NotImplementedException();
        }
    }

    private class TopicsProvider : ITopicsProvider
    {
        public IReadOnlyCollection<FetchAndLockRequest.Topic> GetTopics()
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
