// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CamundaWorkerServiceCollectionExtensionsTest
    {
        [Fact]
        public void TestAddCamundaWorker()
        {
            var services = new Mock<IServiceCollection>().Object;

            services.AddCamundaWorker();
        }
    }
}
