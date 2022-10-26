using System.Xml.Linq;

namespace Camunda.Worker.Variables;

public record XmlVariable(XDocument Value) : VariableBase;
