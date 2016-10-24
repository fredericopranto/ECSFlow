using ECSFlowAttributes;
using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// An explicit exception channel (channel, for short) is an abstract duct through which exceptions 
    /// flow from a raising site to a handling site.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionChannelAttribute : Attribute, IECSFlowAttribute
    {
        public string _name;
        public string _exception;
        public string _raiseSite;


        public ExceptionChannelAttribute(string name, string exception, string raiseSite)
        {
            this._name = name;
            this._exception = exception;
            this._raiseSite = raiseSite;
        }
    }
}