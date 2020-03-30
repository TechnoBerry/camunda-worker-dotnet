using System.Collections.Generic;

namespace Camunda.Worker.Client
{
    public class BpmnErrorRequest
    {
        public BpmnErrorRequest(string workerId, string errorCode, string errorMessage)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            ErrorCode = Guard.NotEmptyAndNotNull(errorCode, nameof(errorCode));
            ErrorMessage = Guard.NotEmptyAndNotNull(errorMessage, nameof(errorMessage));
        }

        public string WorkerId { get; }

        public string ErrorCode { get; }

        public string ErrorMessage { get; }

        public IDictionary<string, Variable>? Variables { get; set; }
    }
}
