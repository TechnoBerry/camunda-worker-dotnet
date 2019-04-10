#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IHandlerFactoryProvider _handlerFactoryProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ILogger<ExternalTaskRouter> _logger;

        public ExternalTaskRouter(IHandlerFactoryProvider handlerFactoryProvider,
            IExceptionHandler exceptionHandler,
            ILogger<ExternalTaskRouter> logger = null)
        {
            _handlerFactoryProvider = Guard.NotNull(handlerFactoryProvider, nameof(handlerFactoryProvider));
            _exceptionHandler = Guard.NotNull(exceptionHandler, nameof(exceptionHandler));
            _logger = logger ?? new NullLogger<ExternalTaskRouter>();
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var externalTask = context.Task;
            var handlerFactory = _handlerFactoryProvider.GetHandlerFactory(externalTask);
            var handler = handlerFactory(context.ServiceProvider);

            _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);

            var executionResult = await ProcessSafe(handler, externalTask);
            await executionResult.ExecuteResultAsync(context);

            _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);
        }

        private async Task<IExecutionResult> ProcessSafe(IExternalTaskHandler handler, ExternalTask task)
        {
            try
            {
                var result = await handler.Process(task);
                return result;
            }
            catch (Exception e)
            {
                if (_exceptionHandler.TryTransformToResult(e, out var result))
                {
                    return result;
                }

                throw;
            }
        }
    }
}
