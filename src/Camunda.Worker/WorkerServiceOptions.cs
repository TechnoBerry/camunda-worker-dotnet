using System.Collections.Generic;
using Camunda.Worker.Execution;

namespace Camunda.Worker
{
    public class WorkerServiceOptions
    {
        internal WorkerServiceOptions(string workerId, IEnumerable<HandlerDescriptor> handlerDescriptors)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            HandlerDescriptors = Guard.NotNull(handlerDescriptors, nameof(handlerDescriptors));
        }

        public string WorkerId { get; }

        public IEnumerable<HandlerDescriptor> HandlerDescriptors { get; }
    }
}
