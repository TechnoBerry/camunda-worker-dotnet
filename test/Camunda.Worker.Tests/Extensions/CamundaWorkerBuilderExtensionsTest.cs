#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker.Extensions
{
    public class CamundaWorkerBuilderExtensionsTest
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly Mock<ICamundaWorkerBuilder> _builderMock = new Mock<ICamundaWorkerBuilder>();

        public CamundaWorkerBuilderExtensionsTest()
        {
            _builderMock.SetupGet(builder => builder.Services).Returns(_services);
        }

        [Fact]
        public void TestAddHandlerWithAttributes()
        {
            var savedDescriptors = new List<HandlerDescriptor>();

            _builderMock
                .Setup(builder => builder.AddHandlerDescriptor(It.IsAny<HandlerDescriptor>()))
                .Callback((HandlerDescriptor descriptor) => savedDescriptors.Add(descriptor))
                .Returns(_builderMock.Object);

            _builderMock.Object.AddHandler<HandlerWithTopics>();

            _builderMock.Verify(builder => builder.AddHandlerDescriptor(It.IsAny<HandlerDescriptor>()),
                Times.Exactly(2));
            Assert.Contains(_services, d => d.Lifetime == ServiceLifetime.Scoped &&
                                            d.ServiceType == typeof(HandlerWithTopics));

            Assert.Equal(2, savedDescriptors.Count);
            Assert.Equal(savedDescriptors[0].Variables, savedDescriptors[1].Variables);
            Assert.NotNull(savedDescriptors[0].Variables);
            Assert.Single(savedDescriptors[0].Variables);
            Assert.Contains(savedDescriptors[0].Variables, v => v == "testVariable");
        }

        [Fact]
        public void TestAddHandlerWithoutTopic()
        {
            _builderMock
                .Setup(builder => builder.AddHandlerDescriptor(It.IsAny<HandlerDescriptor>()))
                .Returns(_builderMock.Object);

            Assert.Throws<Exception>(() => _builderMock.Object.AddHandler<HandlerWithoutTopics>());
        }

        [HandlerTopics("testTopic_1", "testTopic_1")]
        [HandlerVariables("testVariable", LocalVariables = true)]
        private class HandlerWithTopics : IExternalTaskHandler
        {
            public Task<IExecutionResult> Process(ExternalTask externalTask)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult(externalTask.Variables));
            }
        }

        private class HandlerWithoutTopics : IExternalTaskHandler
        {
            public Task<IExecutionResult> Process(ExternalTask externalTask)
            {
                return Task.FromResult<IExecutionResult>(new CompleteResult(externalTask.Variables));
            }
        }
    }
}
