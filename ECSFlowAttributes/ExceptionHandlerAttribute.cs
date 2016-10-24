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
        public string _channel;
        public string _target;
        public string _exception;
        public HandlerDelegate _delegate;


        public ExceptionHandlerAttribute(string channel, string target, string exception, string delegateName)
        {
            this._channel = channel;
            this._target = target;
            this._exception = exception;
            this._delegate = (HandlerDelegate) Delegate.CreateDelegate(Type.GetType("AssemblyToProcessMapping"), 
                                                                       Type.GetType("AssemblyToProcessMapping").GetMethod(delegateName));
        }
    }
}