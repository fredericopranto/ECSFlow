using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// When a class or program cannot handle all the exceptions that flow through an explicit
    /// exception channel, it is necessary to declare these exceptions in the channel’s exception
    /// interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionInterfaceAttribute : Attribute, IECSFlowAttribute
    {
        public ExceptionInterfaceAttribute(string channel, string raiseSiteName, bool isNamespace, string exception)
        {
            this.Exception = exception;
            this.Channel = channel;
            this.RaiseSiteName = raiseSiteName;
            this.IsNamespace = isNamespace;
        }

        public string Exception { get; set; }

        public string Channel { get; set; }

        public string RaiseSiteName { get; set; }

        public bool IsNamespace { get; set; }
    }
}