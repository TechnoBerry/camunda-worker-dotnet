using System;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Camunda.Worker.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Camunda.Worker;

public class ServiceCollectionExtensionsTest
{
    [Theory]
    [InlineData("testWorker")]
    public void Should_RegisterRequiredKeyedServices(string workerId)
    {
        var services = new ServiceCollection();

        services.AddCamundaWorker(workerId, 100);

        Assert.Contains(services, IsRegistered(typeof(IEndpointResolver), ServiceLifetime.Singleton, workerId));
        Assert.Contains(services, IsRegistered(typeof(ICamundaWorker), ServiceLifetime.Transient, workerId));
        Assert.Contains(services, IsRegistered(typeof(IExternalTaskProcessingService), ServiceLifetime.Singleton, workerId));
        Assert.Contains(services, IsRegistered(typeof(IFetchAndLockRequestProvider), ServiceLifetime.Singleton, workerId));

        // IEqternalTaskClient should be registered separately
        services.AddExternalTaskClient(options =>
        {
            options.BaseAddress = new Uri("http://test");
        });

        using var provider = services.BuildServiceProvider();
        var registeredWorker = provider.GetRequiredKeyedService<ICamundaWorker>(workerId);
    }

    private static Predicate<ServiceDescriptor> IsRegistered(Type serviceType, ServiceLifetime lifetime, string workerId)
        => descriptor => descriptor.Lifetime == lifetime &&
                         descriptor.ServiceType == serviceType &&
                         descriptor.IsKeyedService &&
                         workerId.Equals(descriptor.ServiceKey);
}
