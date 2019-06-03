using System;
using System.Collections.Generic;

namespace Camunda.Worker
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VariablesAttribute : Attribute
    {
        public VariablesAttribute(params string[] variables)
        {
            Variables = variables;
        }

        public IReadOnlyList<string> Variables { get; }

        public bool LocalVariables { get; set; }
    }
}
