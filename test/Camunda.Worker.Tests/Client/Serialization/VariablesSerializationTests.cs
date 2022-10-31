using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Camunda.Worker.Variables;
using Snapshooter;
using Snapshooter.Xunit;
using Xunit;

namespace Camunda.Worker.Client.Serialization;

public class VariablesSerializationTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public VariablesSerializationTests()
    {
        _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
            .Also(opts =>
            {
                opts.Converters.Add(new JsonStringEnumConverter());
                opts.Converters.Add(new VariableJsonConverter());
                opts.Converters.Add(new JsonVariableJsonConverter());
                opts.Converters.Add(new XmlVariableJsonConverter());
            });
    }

    [Theory]
    [MemberData(nameof(GetVariables))]
    public void ShouldSerialize(string caseDescriptor, VariableBase variable)
    {
        // Act
        var result = JsonSerializer.Serialize(variable, _jsonSerializerOptions);

        // Assert
        Snapshot.Match(result, SnapshotNameExtension.Create(caseDescriptor));
    }

    public static IEnumerable<object[]> GetVariables()
    {
        yield return new object[] { "Bool1", new BooleanVariable(true) };
        yield return new object[] { "Bool2", new BooleanVariable(false) };
        yield return new object[] { "Double1", new DoubleVariable(double.Epsilon) };
        yield return new object[] { "Double2", new DoubleVariable(0) };
        yield return new object[] { "Double3", new DoubleVariable(Math.PI) };
        yield return new object[] { "Int1", new IntegerVariable(int.MinValue) };
        yield return new object[] { "Int2", new IntegerVariable(int.MaxValue) };
        yield return new object[] { "Short1", new ShortVariable(short.MinValue) };
        yield return new object[] { "Short2", new ShortVariable(short.MaxValue) };
        yield return new object[] { "Long1", new LongVariable(long.MinValue) };
        yield return new object[] { "Long2", new LongVariable(long.MaxValue) };
        yield return new object[] { "String", new StringVariable("\"Hello!\"") };
        yield return new object[] { "Bytes", new BytesVariable(Encoding.UTF8.GetBytes("hello")) };
        yield return new object[] { "Json", new JsonVariable(new JsonObject
        {
            ["Username"] = "John",
            ["Roles"] = new JsonArray
            {
                "Admin"
            }
        }) };
        yield return new object[] { "Xml", new XmlVariable(new XDocument(
            new XElement("username", "John")
        )) };
    }

    [Theory]
    [MemberData(nameof(GetVariableValues))]
    public void ShouldDeserialize(string caseDescriptor, string variableValue)
    {
        // Act
        var result = JsonSerializer.Deserialize<VariableBase>(variableValue, _jsonSerializerOptions);

        // Assert
        Snapshot.Match(result, SnapshotNameExtension.Create(caseDescriptor));
    }

    public static IEnumerable<object[]> GetVariableValues()
    {
        yield return new object[] { "Bool1", @"{""value"":true,""type"":""Boolean""}" };
        yield return new object[] { "Bool2", @"{""value"":false,""type"":""Boolean""}" };
        yield return new object[] { "Double1", @"{""value"":5E-324,""type"":""Double""}" };
        yield return new object[] { "Double2", @"{""value"":0,""type"":""Double""}" };
        yield return new object[] { "Double3", @"{""value"":3.141592653589793,""type"":""Double""}" };
        yield return new object[] { "Int1", @"{""value"":-2147483648,""type"":""Integer""}" };
        yield return new object[] { "Int2", @"{""value"":2147483647,""type"":""Integer""}" };
        yield return new object[] { "Short1", @"{""value"":-32768,""type"":""Short""}" };
        yield return new object[] { "Short2", @"{""value"":32767,""type"":""Short""}" };
        yield return new object[] { "Long1", @"{""value"":-9223372036854775808,""type"":""Long""}" };
        yield return new object[] { "Long2", @"{""value"":9223372036854775807,""type"":""Long""}" };
        yield return new object[] { "String", @"{""value"":""\u0022Hello!\u0022"",""type"":""String""}" };
        yield return new object[] { "Bytes", @"{""value"":""aGVsbG8="",""type"":""Bytes""}" };
        yield return new object[] { "Json", @"{""value"":""{\u0022Username\u0022:\u0022John\u0022,\u0022Roles\u0022:[\u0022Admin\u0022]}"",""type"":""Json""}" };
        yield return new object[] { "Xml", @"{""value"":""\u003Cusername\u003EJohn\u003C/username\u003E"",""type"":""Xml""}" };
    }
}
