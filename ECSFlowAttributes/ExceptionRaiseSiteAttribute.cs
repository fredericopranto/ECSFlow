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
        private string _name;
        private string _target;

        public ExceptionRaiseSiteAttribute(string name, string target)
        {
            this._name = name;
            this._target = target;
        }
    }
}