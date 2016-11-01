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
        private string name;
        private string exception;
        private string raiseSite;

        public ExceptionChannelAttribute(string name, string exception, string raiseSite)
        {
            this.Name = name;
            this.Exception = exception;
            this.RaiseSite = raiseSite;
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

        public string Exception
        {
            get
            {
                return exception;
            }

            set
            {
                exception = value;
            }
        }

        public string RaiseSite
        {
            get
            {
                return raiseSite;
            }

            set
            {
                raiseSite = value;
            }
        }
    }
}