#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


using System.Collections.Generic;

namespace Camunda.Worker.Execution
{
    public sealed class HandlerDescriptor
    {
        public HandlerDescriptor(HandlerFactory factory, HandlerMetadata metadata)
        {
            Factory = Guard.NotNull(factory, nameof(factory));
            Metadata = Guard.NotNull(metadata, nameof(metadata));
        }

        public HandlerFactory Factory { get; }

        public HandlerMetadata Metadata { get; }

        public bool LocalVariables => Metadata.LocalVariables;

        public IEnumerable<string> Variables => Metadata.Variables;

        public int LockDuration => Metadata.LockDuration;
    }
}
