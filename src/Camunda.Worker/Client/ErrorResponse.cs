#region LICENSE

// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#endregion


namespace Camunda.Worker.Client
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
