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
        private string[] exceptionList;
        private Type type;

        public ExceptionChannelAttribute(Type type, string[] exceptionList)
        {
            this.type = type;
            this.exceptionList = exceptionList;
        }
    }
}