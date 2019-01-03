// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Execution;
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

            var savedDescriptors = new List<HandlerDescriptor>();

            builderMock
                .Setup(builder => builder.Add(It.IsAny<HandlerDescriptor>()))
                .Callback((HandlerDescriptor descriptor) => savedDescriptors.Add(descriptor))
                .Returns(builderMock.Object);

            builderMock.SetupGet(builder => builder.Services).Returns(services);

            builderMock.Object.AddHandler<HandlerWithTopics>();

            builderMock.Verify(builder => builder.Add(It.IsAny<HandlerDescriptor>()), Times.Exactly(2));
            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Scoped &&
                                           d.ServiceType == typeof(HandlerWithTopics));

            Assert.Equal(2, savedDescriptors.Count);
            Assert.Equal(savedDescriptors[0].Variables, savedDescriptors[1].Variables);
            Assert.NotNull(savedDescriptors[0].Variables);
            Assert.Single(savedDescriptors[0].Variables);
            Assert.Contains(savedDescriptors[0].Variables, v => v == "testVariable");
        }

        [HandlerTopic("testTopic_1")]
        [HandlerTopic("testTopic_2")]
        [HandlerVariables("testVariable", LocalVariables = true)]
        private class HandlerWithTopics : IExternalTaskHandler
        {
            public Task<IExecutionResult> Process(ExternalTask externalTask,
                CancellationToken cancellationToken)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult(externalTask.Variables));
            }
        }
    }
}
