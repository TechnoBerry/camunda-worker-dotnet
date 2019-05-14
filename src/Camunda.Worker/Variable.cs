using System;
using System.Diagnostics.CodeAnalysis;
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
        public static Variable Bool(bool value) => new Variable(value, VariableType.Boolean);

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(short value) : this(value, VariableType.Short)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Short(short value) => new Variable(value, VariableType.Short);

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(int value) : this(value, VariableType.Integer)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Int(int value) => new Variable(value, VariableType.Integer);

        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(long value) : this(value, VariableType.Long)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Long(long value) => new Variable(value, VariableType.Long);


        [Obsolete("Will be removed after `0.8.0` release")]
        [ExcludeFromCodeCoverage]
        public Variable(double value) : this(value, VariableType.Double)
        {
        }

        [ExcludeFromCodeCoverage]
        public static Variable Double(double value) => new Variable(value, VariableType.Double);

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
