using System;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Camunda.Worker;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddCamundaWorker()
    {
        var services = new ServiceCollection();

        services.AddCamundaWorker("testWorker", 100);

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IOptions<FetchAndLockOptions>>()?.Value);
        Assert.NotNull(provider.GetService<IOptions<WorkerEvents>>()?.Value);

        Assert.Contains(services, IsRegistered(typeof(IEndpointProvider), ServiceLifetime.Singleton));
        Assert.Contains(services, IsRegistered(typeof(ITopicsProvider), ServiceLifetime.Transient));
        Assert.Contains(services, IsRegistered(typeof(ICamundaWorker), ServiceLifetime.Transient));
        Assert.Contains(services, IsRegistered(typeof(WorkerHandlerDescriptor), ServiceLifetime.Singleton));
        Assert.Contains(services, IsRegistered(typeof(IFetchAndLockRequestProvider), ServiceLifetime.Singleton));
    }

    private static Predicate<ServiceDescriptor> IsRegistered(Type serviceType, ServiceLifetime lifetime)
        => descriptor => descriptor.Lifetime == lifetime &&
                         descriptor.ServiceType == serviceType;
}
