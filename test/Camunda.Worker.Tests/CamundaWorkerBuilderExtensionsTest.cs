using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilderExtensionsTest
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly Mock<ICamundaWorkerBuilder> _builderMock = new();

        public CamundaWorkerBuilderExtensionsTest()
        {
            _builderMock.SetupGet(builder => builder.Services).Returns(_services);
        }

        [Fact]
        public void TestAddHandlerWithAttributes()
        {
            // Arrange
            var savedMetadata = new List<HandlerMetadata>();

            _builderMock
                .Setup(builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()))
                .Callback((ExternalTaskDelegate _, HandlerMetadata metadata) => savedMetadata.Add(metadata))
                .Returns(_builderMock.Object);

            // Act
            _builderMock.Object.AddHandler<HandlerWithTopics>();

            // Assert
            _builderMock.Verify(
                builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()),
                Times.Once());
            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Transient &&
                                            d.ServiceType == typeof(HandlerWithTopics));

            var metadata = Assert.Single(savedMetadata);
            Assert.NotNull(metadata);
            var variableName = Assert.Single(metadata.Variables);
            Assert.Equal("testVariable", variableName);
        }

        [Fact]
        public void TestAddHandlerWithAttributesAndMetadataCustomization()
        {
            // Arrange
            var savedMetadata = new List<HandlerMetadata>();

            _builderMock
                .Setup(builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()))
                .Callback((ExternalTaskDelegate _, HandlerMetadata metadata) => savedMetadata.Add(metadata))
                .Returns(_builderMock.Object);

            // Act
            _builderMock.Object.AddHandler<HandlerWithTopics>(m =>
            {
                m.TenantIds = new[] {"myTenant"};
            });

            // Assert
            _builderMock.Verify(
                builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()),
                Times.Once());
            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Transient &&
                                            d.ServiceType == typeof(HandlerWithTopics));

            var metadata = Assert.Single(savedMetadata);
            Assert.NotNull(metadata);
            var variableName = Assert.Single(metadata.Variables);
            Assert.Equal("testVariable", variableName);
            Assert.Equal("myTenant", metadata.TenantIds![0]);
        }

        [Fact]
        public void TestAddHandlerWithoutTopic()
        {
            // Arrange
            _builderMock
                .Setup(builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()))
                .Returns(_builderMock.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => _builderMock.Object.AddHandler<HandlerWithoutTopics>());
        }

        [Fact]
        public void TestAddHandlerWithAllVariables()
        {
            // Arrange
            var savedMetadata = new List<HandlerMetadata>();

            _builderMock
                .Setup(builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()))
                .Callback((ExternalTaskDelegate _, HandlerMetadata metadata) => savedMetadata.Add(metadata))
                .Returns(_builderMock.Object);

            // Act
            _builderMock.Object.AddHandler<HandlerWithAllVariables>();

            // Assert
            _builderMock.Verify(
                builder => builder.AddHandler(It.IsAny<ExternalTaskDelegate>(), It.IsAny<HandlerMetadata>()),
                Times.Once());
            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Transient &&
                                            d.ServiceType == typeof(HandlerWithAllVariables));

            var metadata = Assert.Single(savedMetadata);
            Assert.NotNull(metadata);
            Assert.Null(metadata.Variables);
        }

        [HandlerTopics("testTopic")]
        [HandlerVariables(AllVariables = true)]
        private class HandlerWithAllVariables : IExternalTaskHandler
        {
            public Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult
                {
                    Variables = externalTask.Variables
                });
            }
        }

        [HandlerTopics("testTopic_1", "testTopic_1")]
        [HandlerVariables("testVariable", LocalVariables = true)]
        private class HandlerWithTopics : IExternalTaskHandler
        {
            public Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult
                {
                    Variables = externalTask.Variables
                });
            }
        }

        private class HandlerWithoutTopics : IExternalTaskHandler
        {
            public Task<IExecutionResult> HandleAsync(ExternalTask externalTask, CancellationToken cancellationToken)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult
                {
                    Variables = externalTask.Variables
                });
            }
        }
    }
}
