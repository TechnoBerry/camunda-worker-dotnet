using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Camunda.Worker
{
    public class Variable
    {
        [ExcludeFromCodeCoverage]
        public Variable(bool value) : this(value, VariableType.Boolean)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(short value) : this(value, VariableType.Short)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(int value) : this(value, VariableType.Integer)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(long value) : this(value, VariableType.Long)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(double value) : this(value, VariableType.Double)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(string value) : this(value, VariableType.String)
        {
        }

        [ExcludeFromCodeCoverage]
        public Variable(byte[] value) : this(Convert.ToBase64String(value), VariableType.Bytes)
        {
        }

        [JsonConstructor]
        public Variable(object value, VariableType type)
        {
            Value = value;
            Type = type;
        }

        public object Value { get; }
        public VariableType Type { get; }
    }
}
