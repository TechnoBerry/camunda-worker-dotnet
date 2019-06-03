using System;

namespace Camunda.Worker
{
    [Obsolete("Will be removed after `0.8.0` release use \"VariablesAttribute\" instead")]
    [AttributeUsage(AttributeTargets.Class)]
    public class HandlerVariablesAttribute : VariablesAttribute
    {
        public HandlerVariablesAttribute(params string[] variables) : base(variables)
        {
        }
    }
}
