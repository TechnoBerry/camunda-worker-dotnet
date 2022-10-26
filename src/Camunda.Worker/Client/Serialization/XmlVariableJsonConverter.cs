using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Camunda.Worker.Variables;

namespace Camunda.Worker.Client.Serialization;

public class XmlVariableJsonConverter : JsonConverter<XmlVariable>
{
    public override XmlVariable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;
        var serializedXmlValue = rootElement.GetProperty("value").GetString()
                                  ?? throw new JsonException();

        var deserializedValue = XDocument.Parse(serializedXmlValue);

        return new XmlVariable(deserializedValue);
    }

    public override void Write(Utf8JsonWriter writer, XmlVariable value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("value", value.Value.ToString(SaveOptions.DisableFormatting));
        writer.WriteEndObject();
    }
}
