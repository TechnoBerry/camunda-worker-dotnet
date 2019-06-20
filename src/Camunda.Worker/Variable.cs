using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Camunda.Worker
{
    public class Variable : IEquatable<Variable>
    {
        [ExcludeFromCodeCoverage]
        public static Variable Boolean(bool value) => new Variable(value, VariableType.Boolean);

        public bool AsBoolean()
        {
            EnsureIsOfType(VariableType.Boolean);
            return Convert.ToBoolean(Value);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Short(short value) => new Variable(value, VariableType.Short);

        public short AsShort()
        {
            EnsureIsOfType(VariableType.Short);
            return Convert.ToInt16(Value);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Integer(int value) => new Variable(value, VariableType.Integer);

        public int AsInteger()
        {
            EnsureIsOfType(VariableType.Integer);
            return Convert.ToInt32(Value);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Long(long value) => new Variable(value, VariableType.Long);

        [ExcludeFromCodeCoverage]
        public long AsLong()
        {
            EnsureIsOfType(VariableType.Long);
            return Convert.ToInt64(Value);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Double(double value) => new Variable(value, VariableType.Double);

        public double AsDouble()
        {
            EnsureIsOfType(VariableType.Double);
            return Convert.ToDouble(Value);
        }

        public string AsString()
        {
            EnsureIsOfType(VariableType.String);
            return Convert.ToString(Value);
        }

        [ExcludeFromCodeCoverage]
        public static Variable String(string value) => new Variable(value, VariableType.String);

        [ExcludeFromCodeCoverage]
        public static Variable Bytes(byte[] value) => new Variable(Convert.ToBase64String(value), VariableType.Bytes);

        public byte[] AsBytes()
        {
            EnsureIsOfType(VariableType.Bytes);
            var stringValue = Convert.ToString(Value);
            return Convert.FromBase64String(stringValue);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Json(JObject value) => new Variable(value.ToString(Formatting.None), VariableType.Json);

        [ExcludeFromCodeCoverage]
        public static Variable Json(object value) =>
            new Variable(JsonConvert.SerializeObject(value), VariableType.Json);

        [ExcludeFromCodeCoverage]
        public static Variable Json(object value, JsonSerializerSettings settings) =>
            new Variable(JsonConvert.SerializeObject(value, settings), VariableType.Json);

        public T AsJson<T>(JsonSerializerSettings settings = null)
        {
            EnsureIsOfType(VariableType.Json);
            var stringValue = Convert.ToString(Value);
            return JsonConvert.DeserializeObject<T>(stringValue, settings);
        }

        [ExcludeFromCodeCoverage]
        public static Variable Null() => new Variable(null, VariableType.Null);

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

        [ExcludeFromCodeCoverage]
        public bool Equals(Variable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value) && Type == other.Type;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Variable) obj);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (int) Type;
            }
        }
    }
}
