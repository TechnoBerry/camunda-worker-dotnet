// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace Camunda.Worker
{
    public class CamundaWorkerOptions
    {
        public string WorkerId { get; set; }
        public Uri BaseUri { get; set; }
    }
}
