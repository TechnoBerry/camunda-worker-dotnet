namespace Camunda.Worker.Variables;

public sealed record BytesVariable(byte[] Value) : VariableBase;
