#region LICENSE
// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
#endregion


using System;

namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        private string _workerId = "camunda-worker";
        private int _workerCount = 1;
        private int _asyncResponseTimeout = 10_000;

        public string WorkerId
        {
            get => _workerId;
            set => _workerId = Guard.NotNull(value, nameof(WorkerId));
        }

        public int WorkerCount
        {
            get => _workerCount;
            set => _workerCount = Guard.GreaterThanOrEqual(value, 1, nameof(WorkerCount));
        }

        public Uri BaseUri { get; set; }

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set => _asyncResponseTimeout = Guard.GreaterThanOrEqual(value, 0, nameof(AsyncResponseTimeout));
        }
    }
}
