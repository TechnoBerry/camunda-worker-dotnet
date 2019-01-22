// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Camunda.Worker.Api;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask externalTask,
            ICamundaApiClient camundaApiClient)
        {
            ExternalTask = externalTask;
            CamundaApiClient = camundaApiClient;
        }

        public ExternalTask ExternalTask { get; set; }
        public ICamundaApiClient CamundaApiClient { get; set; }
    }
}
