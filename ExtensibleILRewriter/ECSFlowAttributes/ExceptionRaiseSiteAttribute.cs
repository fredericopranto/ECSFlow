using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then flow it explicit.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionRaiseSiteAttribute : Attribute, IECSFlowAttribute
    {
        public ExceptionRaiseSiteAttribute(string name, string target)
        {
            this.Name = name;
            this.Target = target;
        }

        public string Name { get; set; }

        public string Target { get; set; }
    }
}