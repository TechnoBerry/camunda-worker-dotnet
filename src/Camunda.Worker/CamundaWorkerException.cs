#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;
using System.Runtime.Serialization;

namespace Camunda.Worker
{
    public class CamundaWorkerException : Exception
    {
        protected CamundaWorkerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CamundaWorkerException(string message) : base(message)
        {
        }

        public CamundaWorkerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
