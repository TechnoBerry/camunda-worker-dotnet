using System;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker;

internal static partial class Log
{
    #region Worker

    [LoggerMessage(LogLevel.Debug, "Waiting for external task")]
    public static partial void LogWorker_Waiting(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, "Locked {Count} external tasks")]
    public static partial void LogWorker_Locked(this ILogger logger, int count);

    [LoggerMessage(LogLevel.Warning, "Failed locking of external tasks. Reason: {Reason}")]
    public static partial void LogWorker_FailedLocking(this ILogger logger, string reason, Exception e);

    [LoggerMessage(LogLevel.Warning, "Failed execution of task {ExternalTaskId}")]
    public static partial void LogWorker_FailedExecution(this ILogger logger, string externalTaskId, Exception e);

    #endregion

    #region Invoker

    [LoggerMessage(LogLevel.Debug, "Started processing of task {ExternalTaskId}")]
    public static partial void LogInvoker_StartedProcessing(this ILogger logger, string externalTaskId);

    [LoggerMessage(LogLevel.Debug, "Finished processing of task {ExternalTaskId}")]
    public static partial void LogInvoker_FinishedProcessing(this ILogger logger, string externalTaskId);

    [LoggerMessage(LogLevel.Error, "Failed processing of task {ExternalTaskId}")]
    public static partial void LogInvoker_FailedProcessing(this ILogger logger, string externalTaskId, Exception e);

    #endregion

    [LoggerMessage(LogLevel.Warning, "Failed completion of task {ExternalTaskId}. Reason: {Reason}")]
    public static partial void LogResult_FailedCompletion(
        this ILogger logger,
        string externalTaskId,
        string reason,
        Exception e
    );
}
