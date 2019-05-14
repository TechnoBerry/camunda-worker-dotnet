using System;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Camunda.Worker
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void TestAddCamundaWorker()
        {
            var services = new ServiceCollection();

            services.AddCamundaWorker(options =>
            {
                options.WorkerId = "testWorker";
                options.BaseUri = new Uri("http://test/engine-rest");
            });

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(IConfigureOptions<CamundaWorkerOptions>));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IHandlerFactoryProvider));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskRouter));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExceptionHandler));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ITopicsProvider));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ICamundaWorker));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskSelector));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskClient));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IContextFactory));
        }
    }
}
