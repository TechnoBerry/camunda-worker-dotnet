// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Camunda.Worker.Core
{
    public class DefaultExternalTaskExecutor : IExternalTaskExecutor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHandlerFactoryProvider _handlerFactoryProvider;

        public DefaultExternalTaskExecutor(IServiceScopeFactory scopeFactory,
            IHandlerFactoryProvider handlerFactoryProvider)
        {
            _scopeFactory = scopeFactory;
            _handlerFactoryProvider = handlerFactoryProvider;
        }

        public Task<ExecutionResult> Execute(ExternalTask externalTask) =>
            Execute(externalTask, CancellationToken.None);

        public async Task<ExecutionResult> Execute(ExternalTask externalTask, CancellationToken cancellationToken)
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
                var result = await handler.ProcessSafe(externalTask, cancellationToken);
                return result;
            }
        }
    }
}
