// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Camunda.Worker.Core
{
    public class DefaultExternalTaskExecutor : IExternalTaskExecutor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHandlerFactoryProvider _handlerFactoryProvider;
        private readonly ILogger<DefaultExternalTaskExecutor> _logger;

        public DefaultExternalTaskExecutor(IServiceScopeFactory scopeFactory,
            IHandlerFactoryProvider handlerFactoryProvider,
            ILogger<DefaultExternalTaskExecutor> logger)
        {
            _scopeFactory = scopeFactory;
            _handlerFactoryProvider = handlerFactoryProvider;
            _logger = logger;
        }

        public Task<IExecutionResult> Execute(ExternalTask externalTask) =>
            Execute(externalTask, CancellationToken.None);

        public async Task<IExecutionResult> Execute(ExternalTask externalTask, CancellationToken cancellationToken)
        {
            if (externalTask == null)
            {
                throw new ArgumentNullException(nameof(externalTask));
            }

            var topicName = externalTask.TopicName;

            var handlerFactory = _handlerFactoryProvider.GetHandlerFactory(topicName);

            using (var scope = _scopeFactory.CreateScope())
            {
                var handler = handlerFactory(scope.ServiceProvider);

                _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);
                var result = await handler.ProcessSafe(externalTask, cancellationToken);
                _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);

                return result;
            }
        }
    }
}
