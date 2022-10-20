using System.Text.Json.Nodes;

namespace Camunda.Worker.Variables;

public record JsonVariable(JsonNode Value) : VariableBase;
