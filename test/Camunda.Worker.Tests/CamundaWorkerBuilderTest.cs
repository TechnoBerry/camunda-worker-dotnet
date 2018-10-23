// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilderTest
    {
        [Fact]
        public void TestAdd()
        {
            var services = new Mock<IServiceCollection>().Object;

            var builder = new CamundaWorkerBuilder(services);

            builder.Add(new HandlerDescriptor("testTopic"));
        }
    }
}
