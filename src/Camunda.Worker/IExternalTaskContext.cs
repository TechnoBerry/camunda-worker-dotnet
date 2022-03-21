using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;

namespace Camunda.Worker;

public interface IExternalTaskContext
{
    ExternalTask Task { get; }

    IExternalTaskClient Client { get; }

    IServiceProvider ServiceProvider { get; }

    CancellationToken ProcessingAborted { get; }

    [Obsolete("Use Client instead")]
    Task ExtendLockAsync(int newDuration);

    [Obsolete("Use Client instead")]
    Task CompleteAsync(
        IDictionary<string, Variable>? variables = null,
        IDictionary<string, Variable>? localVariables = null
    );

    [Obsolete("Use Client instead")]
    Task ReportFailureAsync(
        string? errorMessage,
        string? errorDetails,
        int? retries = default,
        int? retryTimeout = default
    );

    [Obsolete("Use Client instead")]
    Task ReportBpmnErrorAsync(
        string errorCode,
        string errorMessage,
        IDictionary<string, Variable>? variables = null
    );
}
