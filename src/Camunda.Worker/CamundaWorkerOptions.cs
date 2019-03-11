// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        private int _asyncResponseTimeout = 10_000;
        private string _workerId = "camunda-worker";

        public string WorkerId
        {
            get => _workerId;
            set => _workerId = Guard.NotNull(value, nameof(value));
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
