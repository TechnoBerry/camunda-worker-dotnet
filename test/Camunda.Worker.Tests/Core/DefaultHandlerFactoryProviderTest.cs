// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Moq;
using Xunit;

namespace Camunda.Worker.Core
{
    public class DefaultHandlerFactoryProviderTest
    {
        [Fact]
        public void TestGetHandlerFactory()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();

            var provider = new DefaultHandlerFactoryProvider(new []
            {
                new HandlerDescriptor("topic1", p => handlerMock.Object)
            });

            var factory = provider.GetHandlerFactory("topic1");
            Assert.NotNull(factory);
        }
    }
}
