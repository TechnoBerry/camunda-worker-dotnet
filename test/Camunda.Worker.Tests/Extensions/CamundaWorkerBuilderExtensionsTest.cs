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
            var services = new ServiceCollection();
            var builderMock = new Mock<ICamundaWorkerBuilder>();

            builderMock.Setup(builder => builder.Add(It.IsAny<HandlerDescriptor>())).Returns(builderMock.Object);

            builderMock.SetupGet(builder => builder.Services).Returns(services);

            builderMock.Object.AddHandler<HandlerWithTopics>();

            builderMock.Verify(builder => builder.Add(It.IsAny<HandlerDescriptor>()), Times.Exactly(2));
            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Scoped &&
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
