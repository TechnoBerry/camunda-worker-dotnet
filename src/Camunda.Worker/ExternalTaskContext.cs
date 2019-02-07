// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public class ExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask externalTask,
            IExternalTaskCamundaClient externalTaskCamundaClient)
        {
            ExternalTask = Guard.NotNull(externalTask, nameof(externalTask));
            ExternalTaskCamundaClient = Guard.NotNull(externalTaskCamundaClient, nameof(externalTaskCamundaClient));
        }

        public ExternalTask ExternalTask { get; }
        public IExternalTaskCamundaClient ExternalTaskCamundaClient { get; }
    }
}
