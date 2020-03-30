using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Camunda.Worker
{
    [ExcludeFromCodeCoverage]
    public class CamundaWorkerException : Exception
    {
        protected CamundaWorkerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CamundaWorkerException(string? message) : base(message)
        {
        }

        public CamundaWorkerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
