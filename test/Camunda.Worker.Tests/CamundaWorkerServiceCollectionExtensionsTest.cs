// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Core;
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
            var services = new ServiceCollection();

            services.AddCamundaWorker(options =>
            {
                options.WorkerId = "testWorker";
                options.BaseUri = new Uri("http://test/engine-rest");
            });

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(IHandlerFactoryProvider));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskExecutor));
        }
    }
}
