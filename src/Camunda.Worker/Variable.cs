using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Camunda.Worker
{
    public class Variable
    {
        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(bool value) : this(value, VariableType.Boolean)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Boolean(bool value) => new Variable(value, VariableType.Boolean);

        public bool AsBoolean()
        {
            EnsureIsOfType(VariableType.Boolean);
            return Convert.ToBoolean(Value);
        }

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(short value) : this(value, VariableType.Short)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Short(short value) => new Variable(value, VariableType.Short);

        public short AsShort()
        {
            EnsureIsOfType(VariableType.Short);
            return Convert.ToInt16(Value);
        }

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(int value) : this(value, VariableType.Integer)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Integer(int value) => new Variable(value, VariableType.Integer);

        public int AsInteger()
        {
            EnsureIsOfType(VariableType.Integer);
            return Convert.ToInt32(Value);
        }

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(long value) : this(value, VariableType.Long)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Long(long value) => new Variable(value, VariableType.Long);

        [ExcludeFromCodeCoverage]
        public long AsLong()
        {
            EnsureIsOfType(VariableType.Long);
            return Convert.ToInt64(Value);
        }


        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(double value) : this(value, VariableType.Double)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Double(double value) => new Variable(value, VariableType.Double);

        public double AsDouble()
        {
            EnsureIsOfType(VariableType.Double);
            return Convert.ToDouble(Value);
        }

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(string value) : this(value, VariableType.String)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable String(string value) => new Variable(value, VariableType.String);

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(byte[] value) : this(Convert.ToBase64String(value), VariableType.Bytes)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Bytes(byte[] value) => new Variable(Convert.ToBase64String(value), VariableType.Bytes);

        [ExcludeFromCodeCoverage]
        public static Variable Json(JObject value) => new Variable(value.ToString(Formatting.None), VariableType.Json);

        [ExcludeFromCodeCoverage]
        public static Variable Json(object value) => new Variable(JsonConvert.SerializeObject(value), VariableType.Json);

        [ExcludeFromCodeCoverage]
        public static Variable Json(object value, JsonSerializerSettings settings) =>
            new Variable(JsonConvert.SerializeObject(value, settings), VariableType.Json);

        [ExcludeFromCodeCoverage]
        private void EnsureIsOfType(params VariableType[] types)
        {
            if (types.Any(type => type == Type)) return;

            throw new InvalidCastException($"Type {Type} not in [{string.Join(", ", types)}]");
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
