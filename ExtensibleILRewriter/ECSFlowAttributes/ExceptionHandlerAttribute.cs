using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// Pluggable handlers are exception handlers that can be attached to explicit exception channels.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionHandlerAttribute : Attribute, IECSFlowAttribute
    {
        public ExceptionHandlerAttribute(string channel, string target, string exception, string methodName)
        {
            this.Channel = channel;
            this.Target = target;
            this.Exception = exception;
            this.MethodName = methodName;
        }

        public string Channel { get; set; }

        public string Target { get; set; }

        public string Exception { get; set; }

        public string MethodName { get; set; }
    }
}