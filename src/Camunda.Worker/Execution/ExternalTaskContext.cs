// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public class ExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask externalTask,
            IExternalTaskCamundaClient externalTaskCamundaClient)
        {
            ExternalTask = externalTask ?? throw new ArgumentNullException(nameof(externalTask));
            ExternalTaskCamundaClient = externalTaskCamundaClient ?? throw new ArgumentNullException(nameof(externalTaskCamundaClient));
        }

        public ExternalTask ExternalTask { get; }
        public IExternalTaskCamundaClient ExternalTaskCamundaClient { get; }
    }
}
