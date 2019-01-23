// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask externalTask,
            ICamundaApiClient camundaApiClient)
        {
            ExternalTask = externalTask ?? throw new ArgumentNullException(nameof(externalTask));
            CamundaApiClient = camundaApiClient ?? throw new ArgumentNullException(nameof(camundaApiClient));
        }

        public ExternalTask ExternalTask { get; }
        public ICamundaApiClient CamundaApiClient { get; }
    }
}
