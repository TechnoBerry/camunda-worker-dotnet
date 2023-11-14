using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Camunda.Worker.Variables;

namespace Camunda.Worker.Client.Serialization;

public class JsonVariableJsonConverter : JsonConverter<JsonVariable>
{
    public override JsonVariable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;
        var serializedJsonValue = rootElement.GetProperty("value").GetString()
                                  ?? throw new JsonException();

        var deserializedValue = JsonNode.Parse(serializedJsonValue) ?? throw new JsonException();

        return new JsonVariable(deserializedValue);
    }

    public override void Write(Utf8JsonWriter writer, JsonVariable value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("value", value.Value.ToJsonString(options));
        writer.WriteEndObject();
    }
}
