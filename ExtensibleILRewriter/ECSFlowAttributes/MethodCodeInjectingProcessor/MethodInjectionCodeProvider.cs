using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Methods;
using Mono.Cecil;
using System;
using System.Linq;
using System.Reflection;

namespace ECSFlowAttributes
{
    public partial class MethodInjectionCodeProvider : CodeProvider<MethodCodeInjectingCodeProviderArgument>
    {
        public override bool HasState { get { return true; } }

        public FieldDefinition StateField { get; set; }

        public override bool ShouldBeInjected(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var method = codeProviderArgument.Method;
            var methodName = method.Name;
            var methodBaseType = method.DeclaringComponent.Name;

            var handlers = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                           where t.AttributeType.FullName == typeof(ExceptionHandlerAttribute).FullName
                           select t;

            var matchHandlers = from t in handlers
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(methodBaseType, ".", methodName)))
                                select t;

            foreach (var item in matchHandlers)
            {
                return true;
            }

            return false;
        }

        public override Type GetStateType()
        {
            return typeof(State);
        }

        public override MethodInfo GetCodeProvidingMethod(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var method = codeProviderArgument.Method;
            var methodName = method.Name;
            var methodBaseType = method.DeclaringComponent.Name;
            var methodCall = string.Empty;

            var handlers = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                           where t.AttributeType.FullName == typeof(ExceptionHandlerAttribute).FullName
                           select t;

            var matchHandlers = from t in handlers
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(methodBaseType, ".", methodName)))
                                select t;

            foreach (var item in matchHandlers)
            {
                methodCall = item.ConstructorArguments.Last().Value.ToString();
            }

            var parameters = codeProviderArgument.Method.UnderlyingComponent.Parameters;

            var call = GetType().GetMethod(methodCall);
            return call;
        }

        public override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            throw new NotImplementedException();
        }
    }
}
