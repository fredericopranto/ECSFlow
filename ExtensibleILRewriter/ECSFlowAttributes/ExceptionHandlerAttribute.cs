using ECSFlowAttributes;
using System;

namespace ECSFlowAttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionHandlerAttribute : Attribute, IECSFlowAttribute
    {
        private string channel;
        private string target;
        private string exception;
        private string methodName;

        public ExceptionHandlerAttribute(string channel, string target, string exception, string methodName)
        {
            this.Channel = channel;
            this.Target = target;
            this.Exception = exception;
            this.MethodName = methodName;
        }

        public string Channel
        {
            get
            {
                return channel;
            }

            set
            {
                channel = value;
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

        public string MethodName
        {
            get
            {
                return methodName;
            }

            set
            {
                methodName = value;
            }
        }
    }
}