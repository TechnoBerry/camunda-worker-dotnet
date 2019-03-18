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
            set => _workerId = Guard.NotNull(value, nameof(value));
        }

        public int WorkerCount
        {
            get => _workerCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("'WorkerCount' must be greater than or equal to 1");
                }

                _workerCount = value;
            }
        }

        public Uri BaseUri { get; set; }

        public int AsyncResponseTimeout
        {
            get => _asyncResponseTimeout;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("'AsyncResponseTimeout' must be greater than or equal to 0");
                }

                _asyncResponseTimeout = value;
            }
        }
    }
}
