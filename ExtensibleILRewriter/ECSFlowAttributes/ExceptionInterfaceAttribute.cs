using ECSFlowAttributes;
using System;
using System.Collections.Generic;

namespace ECSFlowAttributes
{
    /// <summary>
    /// An explicit exception channel (channel, for short) is an abstract duct through which exceptions 
    /// flow from a raising site to a handling site.
    /// 
    /// When a class or program cannot handle all the exceptions that flow through an explicit
    /// exception channel, it is necessary to declare these exceptions in the channel’s exception
    /// interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ExceptionInterfaceAttribute : Attribute, IECSFlowAttribute
    {
        private string exception;
        private string alias;
        private string channel;
        private string raiseSiteName;
        private bool isNamespace;

        public ExceptionInterfaceAttribute(string channel, string raiseSiteName, bool isNamespace, string exception = null)
        {
            this.Exception = exception;
            this.Channel = channel;
            this.RaiseSiteName = raiseSiteName;
            this.IsNamespace = isNamespace;
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

        public string Alias
        {
            get
            {
                return alias;
            }

            set
            {
                alias = value;
            }
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

        public string RaiseSiteName
        {
            get
            {
                return raiseSiteName;
            }

            set
            {
                raiseSiteName = value;
            }
        }

        public bool IsNamespace
        {
            get
            {
                return isNamespace;
            }

            set
            {
                isNamespace = value;
            }
        }
    }
}