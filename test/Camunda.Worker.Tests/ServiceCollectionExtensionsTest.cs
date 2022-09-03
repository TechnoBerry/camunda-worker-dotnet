using System;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
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

        Assert.NotNull(provider.GetService<IOptionsMonitor<FetchAndLockOptions>>()?.Get("testWorker"));
        Assert.NotNull(provider.GetService<IOptions<WorkerEvents>>()?.Value);

        Assert.Contains(services, IsRegistered(typeof(IEndpointResolver), ServiceLifetime.Singleton));
        Assert.Contains(services, IsRegistered(typeof(ICamundaWorker), ServiceLifetime.Transient));
        Assert.Contains(services, IsRegistered(typeof(WorkerHandlerDescriptor), ServiceLifetime.Singleton));
        Assert.Contains(services, IsRegistered(typeof(IFetchAndLockRequestProvider), ServiceLifetime.Singleton));
    }

    private static Predicate<ServiceDescriptor> IsRegistered(Type serviceType, ServiceLifetime lifetime)
        => descriptor => descriptor.Lifetime == lifetime &&
                         descriptor.ServiceType == serviceType;
}
