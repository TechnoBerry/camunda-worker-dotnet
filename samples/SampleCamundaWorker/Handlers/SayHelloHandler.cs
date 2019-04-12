#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;
using System.Threading.Tasks;
using Camunda.Worker;

namespace SampleCamundaWorker.Handlers
{
    [HandlerTopics("sayHello", LockDuration = 10000)]
    [HandlerVariables("USERNAME")]
    public class SayHelloHandler : IExternalTaskHandler
    {
        public async Task<IExecutionResult> Process(ExternalTask externalTask)
        {
            if (!externalTask.Variables.TryGetValue("USERNAME", out var usernameVariable))
            {
                return new BpmnErrorResult("NO_USER", "Username not provided");
            }

            var username = usernameVariable.Value;

            await Task.Delay(1000);

            return new CompleteResult(new Dictionary<string, Variable>
            {
                ["MESSAGE"] = new Variable($"Hello, {username}!")
            });
        }
    }
}
