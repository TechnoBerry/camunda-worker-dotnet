using System.Text.Json.Nodes;

namespace Camunda.Worker.Variables;

public sealed record UnknownVariable(string Type, JsonNode? Value) : VariableBase;
