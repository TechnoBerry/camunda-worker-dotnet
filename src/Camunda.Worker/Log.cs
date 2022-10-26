using System;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker;

internal static partial class Log
{
    #region Worker

    [LoggerMessage(0, LogLevel.Debug, "Waiting for external task")]
    public static partial void Worker_Waiting(ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Locked {Count} external tasks")]
    public static partial void Worker_Locked(ILogger logger, int count);

    [LoggerMessage(0, LogLevel.Warning, "Failed locking of external tasks. Reason: {Reason}")]
    public static partial void Worker_FailedLocking(ILogger logger, string reason, Exception e);

    [LoggerMessage(0, LogLevel.Warning, "Failed execution of task {ExternalTaskId}")]
    public static partial void Worker_FailedExecution(ILogger logger, string externalTaskId, Exception e);

    #endregion

    #region Invoker

    [LoggerMessage(0, LogLevel.Debug, "Started processing of task {ExternalTaskId}")]
    public static partial void Invoker_StartedProcessing(ILogger logger, string externalTaskId);

    [LoggerMessage(0, LogLevel.Debug, "Finished processing of task {ExternalTaskId}")]
    public static partial void Invoker_FinishedProcessing(ILogger logger, string externalTaskId);

    [LoggerMessage(0, LogLevel.Error, "Failed processing of task {ExternalTaskId}")]
    public static partial void Invoker_FailedProcessing(ILogger logger, string externalTaskId, Exception e);

    #endregion

    [LoggerMessage(0, LogLevel.Warning, "Failed completion of task {ExternalTaskId}. Reason: {Reason}")]
    public static partial void Result_FailedCompletion(
        ILogger logger,
        string externalTaskId,
        string reason,
        Exception e
    );
}
