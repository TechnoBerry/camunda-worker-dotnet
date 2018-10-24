// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace Camunda.Worker.Core
{
    public interface IHandlerFactoryProvider
    {
        Func<IServiceProvider, IExternalTaskHandler> GetHandlerFactory(string topicName);
    }
}
