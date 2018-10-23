// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Extensions
{
    public class CamundaWorkerBuilderExtensionsTest
    {
        [Fact]
        public void TestAddHandlerWithAttributes()
        {
            var descriptors = new List<ServiceDescriptor>();

            var servicesMock = new Mock<IServiceCollection>();
            var builderMock = new Mock<ICamundaWorkerBuilder>();

            servicesMock.Setup(services => services.Add(It.IsAny<ServiceDescriptor>()))
                .Callback((ServiceDescriptor descriptor) => descriptors.Add(descriptor));

            builderMock.Setup(builder => builder.Add(It.IsAny<HandlerDescriptor>())).Returns(builderMock.Object);

            builderMock.SetupGet(builder => builder.Services).Returns(servicesMock.Object);

            builderMock.Object.AddHandler<HandlerWithTopics>();

            builderMock.Verify(builder => builder.Add(It.IsAny<HandlerDescriptor>()), Times.Exactly(2));
            Assert.Contains(descriptors, d => d.Lifetime == ServiceLifetime.Scoped &&
                                              d.ServiceType == typeof(HandlerWithTopics));
        }

        [HandlerTopic("testTopic_1")]
        [HandlerTopic("testTopic_2")]
        private class HandlerWithTopics : IExternalTaskHandler
        {
            public Task<IDictionary<string, Variable>> Process(ExternalTask externalTask,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(externalTask.Variables);
            }
        }
    }
}
