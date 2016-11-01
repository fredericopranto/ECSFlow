using ExtensibleILRewriter.CodeInjection;
using ExtensibleILRewriter.Processors.Methods;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ECSFlowAttributes.MethodCodeInjectingProcessor
{
    public class MethodInjectionCodeProvider : CodeProvider<MethodCodeInjectingCodeProviderArgument>
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
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(methodBaseType, ".", (methodName))))
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

        public static void InjectedMethod_NoValue(State state)
        {
            MessageBox.Show("No Value");
        }

        public static void InjectedMethod_Value(State state, int value)
        {
            MessageBox.Show("Value:" + value);
        }

        public static void OutOfMemoryExceptionHandler(State state, object value)
        {
            MessageBox.Show("OutOfMemoryException caught:" + value.ToString());
        }

        public override CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var parameters = codeProviderArgument.Method.UnderlyingComponent.Parameters;

            if (parameters.Count > 0 && parameters[0].ParameterType.FullName == typeof(int).FullName)
                return new CodeProviderCallArgument[]
                   {
                        CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField),
                        CodeProviderCallArgument.CreateParameterArgument("value", typeof(int), codeProviderArgument.Method.UnderlyingComponent.Parameters[0])
                   };
            else
                return new CodeProviderCallArgument[]
                       {
                            CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField),
                       };
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
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(methodBaseType, ".", (methodName))))
                                select t;

            foreach (var item in matchHandlers)
            {
                methodCall = item.ConstructorArguments.Last().Value.ToString();
            }

            var parameters = codeProviderArgument.Method.UnderlyingComponent.Parameters;

            var call = GetType().GetMethod(methodCall);
            return call;
        }

        public override MethodDefinition GetCodeProvidingMethoDefinition(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            throw new NotImplementedException();
        }
    }

    public class State
    {
        public List<object> Items { get; } = new List<object>();
    }
}
