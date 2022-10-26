using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Camunda.Worker.Variables;

public record JsonVariable(JsonNode Value) : VariableBase
{
    public static JsonVariable Create<T>(T value, JsonSerializerOptions? options = null)
    {
        var jsonNode = JsonSerializer.SerializeToNode(value, options)
                       ?? throw new ArgumentException("Given object could not be serialized to json", nameof(value));

        return new JsonVariable(jsonNode);
    }

    public T? Parse<T>(JsonSerializerOptions? options = null)
    {
        return Value.Deserialize<T>(options);
    }
}
