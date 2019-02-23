// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Camunda.Worker.Client
{
    public class ReportFailureRequest
    {
        public ReportFailureRequest(string workerId)
        {
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
        }

        public string WorkerId { get; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }
}
