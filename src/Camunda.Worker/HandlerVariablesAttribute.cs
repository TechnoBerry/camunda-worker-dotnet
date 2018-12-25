// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HandlerVariablesAttribute : Attribute
    {
        public HandlerVariablesAttribute(params string[] variables)
        {
            Variables = variables;
        }

        public IReadOnlyList<string> Variables { get; }
    }
}
