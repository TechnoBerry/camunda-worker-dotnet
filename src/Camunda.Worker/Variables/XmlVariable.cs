using System.Xml.Linq;

namespace Camunda.Worker.Variables;

public sealed record XmlVariable(XDocument Value) : VariableBase;
