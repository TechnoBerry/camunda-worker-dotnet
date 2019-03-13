// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class TopicBasedFactoryProviderTest
    {
        [Fact]
        public void TestGetKnownHandlerFactory()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();

            var provider = new TopicBasedFactoryProvider(new[]
            {
                new HandlerDescriptor("topic1", p => handlerMock.Object)
            });

            var factory = provider.GetHandlerFactory(new ExternalTask("test", "test", "topic1"));
            Assert.NotNull(factory);
        }

        [Fact]
        public void TestGetUnknownHandlerFactory()
        {
            var provider = new TopicBasedFactoryProvider(Enumerable.Empty<HandlerDescriptor>());
            Assert.Throws<ArgumentException>(
                () => provider.GetHandlerFactory(new ExternalTask("test", "test", "topic1"))
            );
        }
    }
}
