using ECSFlowAttributes;
using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// If an <see cref="Exception"/> occurs in the applied method then flow it explicit.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionRaiseSiteAttribute : Attribute, IECSFlowAttribute
    {
        private string name;
        private string target;

        public ExceptionRaiseSiteAttribute(string name, string target)
        {
            this.Name = name;
            this.Target = target;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }
    }
}