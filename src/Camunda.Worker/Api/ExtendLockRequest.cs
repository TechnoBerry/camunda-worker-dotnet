// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Camunda.Worker.Api
{
    public class ExtendLockRequest
    {
        public string WorkerId { get; set; }
        public int NewDuration { get; set; }
    }
}
