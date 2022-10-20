using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Camunda.Worker.Variables;

namespace Camunda.Worker.Client.Serialization;

public class VariableJsonConverter : JsonConverter<VariableBase>
{
    public override VariableBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;
        var variableType = rootElement.GetProperty("type").GetString();

        return variableType switch
        {
            "String" => rootElement.Deserialize<StringVariable>(options),
            "Boolean" => rootElement.Deserialize<BooleanVariable>(options),
            "Short" => rootElement.Deserialize<ShortVariable>(options),
            "Integer" => rootElement.Deserialize<IntegerVariable>(options),
            "Long" => rootElement.Deserialize<LongVariable>(options),
            "Double" => rootElement.Deserialize<DoubleVariable>(options),
            "Bytes" => rootElement.Deserialize<BytesVariable>(options),
            "Null" => rootElement.Deserialize<NullVariable>(options),
            "Json" => rootElement.Deserialize<JsonVariable>(options),
            _ => rootElement.Deserialize<UnknownVariable>(options)
        };
    }

    public override void Write(Utf8JsonWriter writer, VariableBase value, JsonSerializerOptions options)
    {
        var jsonNode = JsonSerializer.SerializeToNode(value, value.GetType(), options)
                       ?? throw new JsonException();

        jsonNode["type"] = value switch
        {
            StringVariable => "String",
            BooleanVariable => "Boolean",
            ShortVariable => "Short",
            IntegerVariable => "Integer",
            LongVariable => "Long",
            DoubleVariable => "Double",
            BytesVariable => "Bytes",
            NullVariable => "Null",
            JsonVariable => "Json",
            UnknownVariable unknownVariable => unknownVariable.Type,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };

        jsonNode.WriteTo(writer, options);
    }
}
