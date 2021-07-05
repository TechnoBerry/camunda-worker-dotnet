using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camunda.Worker
{
    public interface IExternalTaskContext
    {
        ExternalTask Task { get; }

        IServiceProvider ServiceProvider { get; }

        bool Completed { get; }

        Task ExtendLockAsync(int newDuration);

        Task CompleteAsync(
            IDictionary<string, Variable>? variables = null,
            IDictionary<string, Variable>? localVariables = null
        );

        Task ReportFailureAsync(
            string? errorMessage,
            string? errorDetails,
            int? retries = default,
            int? retryTimeout = default
        );

        Task ReportBpmnErrorAsync(
            string errorCode,
            string errorMessage,
            IDictionary<string, Variable>? variables = null
        );
    }
}
