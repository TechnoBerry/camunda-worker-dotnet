using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilderTest
    {
        private readonly ServiceCollection _services = new();
        private readonly CamundaWorkerBuilder _builder;

        public CamundaWorkerBuilderTest()
        {
            _builder = new CamundaWorkerBuilder(_services, "testWorker");
        }

        [Fact]
        public void TestAddDescriptor()
        {
            Task FakeHandlerDelegate(IExternalTaskContext context) => Task.CompletedTask;

            _builder.AddHandlerDescriptor(new HandlerDescriptor(FakeHandlerDelegate,
                new HandlerMetadata(new[] {"testTopic"})));

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

            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ITopicsProvider) &&
                                           d.ImplementationType == typeof(TopicsProvider));
        }

        [Fact]
        public void TestAddTaskSelector()
        {
            _builder.AddTaskSelector<ExternalTaskSelector>();

            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskSelector) &&
                                           d.ImplementationType == typeof(ExternalTaskSelector));
        }

        [Fact]
        public void TestConfigurePipeline()
        {
            _builder.ConfigurePipeline(pipeline => { });

            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(PipelineDescriptor));
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

        private class ExternalTaskSelector : IExternalTaskSelector
        {
            public Task<IReadOnlyCollection<ExternalTask>> SelectAsync(
                CancellationToken cancellationToken = default
            )
            {
                throw new NotImplementedException();
            }
        }
    }
}
