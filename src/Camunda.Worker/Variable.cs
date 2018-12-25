// Copyright (c) Alexey Malinin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Camunda.Worker
{
    public class Variable
    {
        public Variable(bool value)
        {
            Value = value;
            Type = VariableType.Boolean;
        }

        public Variable(short value)
        {
            Value = value;
            Type = VariableType.Short;
        }

        public Variable(int value)
        {
            Value = value;
            Type = VariableType.Integer;
        }

        public Variable(long value)
        {
            Value = value;
            Type = VariableType.Long;
        }

        public Variable(float value) : this((double) value)
        {
        }

        public Variable(double value)
        {
            Value = value;
            Type = VariableType.Double;
        }

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
