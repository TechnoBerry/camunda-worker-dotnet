// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using Camunda.Worker.Client;

namespace Camunda.Worker.Execution
{
    public interface ITopicsProvider
    {
        IEnumerable<FetchAndLockRequest.Topic> GetTopics();
    }
}
