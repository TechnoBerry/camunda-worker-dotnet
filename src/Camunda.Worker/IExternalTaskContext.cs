#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public interface IExternalTaskContext
    {
        ExternalTask Task { get; }

        Task CompleteAsync(IDictionary<string, Variable> variables,
            IDictionary<string, Variable> localVariables = null);

        Task ReportFailureAsync(string errorMessage, string errorDetails);

        Task ReportBpmnErrorAsync(string errorCode, string errorMessage,
            IDictionary<string, Variable> variables = null);
    }
}
