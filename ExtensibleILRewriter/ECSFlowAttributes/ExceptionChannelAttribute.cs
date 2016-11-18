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
        public ExceptionChannelAttribute(string name, string exception, string raiseSite)
        {
            this.Name = name;
            this.Exception = exception;
            this.RaiseSite = raiseSite;
        }

        public string Name { get; set; }

        public string Exception { get; set; }

        public string RaiseSite { get; set; }
    }
}