#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopics("sayHelloGuest")]
    public class SayHelloGuestHandler : IExternalTaskHandler
    {
        public Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            return Task.FromResult<IExecutionResult>(new CompleteResult(new Dictionary<string, Variable>
            {
                ["MESSAGE"] = new Variable("Hello, Guest!")
            }));
        }
    }
}
