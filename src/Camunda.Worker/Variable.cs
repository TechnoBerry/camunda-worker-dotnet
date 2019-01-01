// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Diagnostics.CodeAnalysis;

namespace Camunda.Worker
{
    public class Variable
    {
        [ExcludeFromCodeCoverage]
        public Variable(bool value)
        {
            Value = value;
            Type = VariableType.Boolean;
        }

        [ExcludeFromCodeCoverage]
        public Variable(short value)
        {
            Value = value;
            Type = VariableType.Short;
        }

        [ExcludeFromCodeCoverage]
        public Variable(int value)
        {
            Value = value;
            Type = VariableType.Integer;
        }

        [ExcludeFromCodeCoverage]
        public Variable(long value)
        {
            Value = value;
            Type = VariableType.Long;
        }

        [ExcludeFromCodeCoverage]
        public Variable(float value) : this((double) value)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(double value)
        {
            Value = value;
            Type = VariableType.Double;
        }

        [ExcludeFromCodeCoverage]
        public Variable(string value)
        {
            Value = value;
            Type = VariableType.String;
        }

        public Variable()
        {
        }

        public object Value { get; set; }
        public VariableType Type { get; set; }
    }
}
