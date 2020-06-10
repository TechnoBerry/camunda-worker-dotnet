using System.Collections.Generic;

using System;
using System.Collections.Generic;

namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        private string _workerId = "camunda-worker";
        private int _workerCount = Constants.MinimumWorkerCount;
        private int _asyncResponseTimeout = 10_000;
        private Dictionary<string, string> _CustomHeaders;

        public string WorkerId
        {
            get => _workerId;
            set => _workerId = Guard.NotNull(value, nameof(WorkerId));
        }

        public int WorkerCount
        {
            get => _workerCount;
            set => _workerCount = Guard.GreaterThanOrEqual(value, Constants.MinimumWorkerCount, nameof(WorkerCount));
        }

        public Uri? BaseUri { get; set; }

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set => _asyncResponseTimeout = Guard.GreaterThanOrEqual(value, 0, nameof(AsyncResponseTimeout));
        }

        public Dictionary<string, string> CustomHttpHeaders
        {
            get => _CustomHeaders;
            set => _CustomHeaders = Guard.NotNull(value, nameof(WorkerId));
        }
    }
}
