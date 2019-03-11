// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Camunda.Worker.Client;

namespace Camunda.Worker
{
    public class ExternalTaskContext
    {
        public ExternalTaskContext(ExternalTask task, IExternalTaskCamundaClient client)
        {
            Task = Guard.NotNull(task, nameof(task));
            Client = Guard.NotNull(client, nameof(client));
        }

        public ExternalTask Task { get; }
        public IExternalTaskCamundaClient Client { get; }
    }
}
