using ECSFlowAttributes;
using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Processors.Methods;
using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.CodeInjection
{
    // TODO - cacheing
    public abstract class CodeProvider<CodeProviderArgumentType>
    {
        public abstract bool HasState { get; }

        public virtual Type GetStateType()
        {
            if (!HasState)
            {
                throw new InvalidOperationException($"Cannot call method '{nameof(GetStateType)}' on code provider which does not have state. Code provider name: '{GetType().FullName}'.");
            }

            throw new NotImplementedException($"Method '{nameof(GetStateType)}' has to be overriden and implemented in order to support state for code provider '{GetType().FullName}'.");
        }

        public abstract bool ShouldBeInjected(CodeProviderArgumentType codeProviderArgument);

        public void CheckCodeProvidingMethodArguments(CodeProviderCallArgument[] requiredParameters)
        {
            var stateParametersCount = requiredParameters.Count(p => p.Type == CodeProviderCallArgumentType.FieldDefinition);

            if (HasState && stateParametersCount == 0)
            {
                throw new InvalidOperationException($"Code provider '{GetType().FullName}' declares it has state but contains zero FieldDefinition required parameters.");
            }

            if (stateParametersCount > 1)
            {
                throw new InvalidOperationException($"Code provider '{GetType().FullName}' contains more than one FieldDefinition required parameter.");
            }
        }

        public List<CodeProviderInjectionInfo> GetCallInfo(MethodCodeInjectingCodeProviderArgument codeProviderArgument, ModuleDefinition destinationModule)
        {
            List<MethodDefinition> methods = GetCodeProvidingMethoDefinition(codeProviderArgument);
            List<CodeProviderInjectionInfo> returnCallInfo = new List<CodeProviderInjectionInfo>();

            foreach (var methodInfo in methods)
            {
                var methodArguments = CodeProviderCallArgument.EmptyCollection;

                /*
                 methodArguments = new CodeProviderCallArgument[]
                           {
                            CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField),
                            CodeProviderCallArgument.CreateParameterArgument("value", typeof(Exception), methodInfo.Parameters[1])
                           };
                 */

                methodArguments = new CodeProviderCallArgument[]
                              {
                        CodeProviderCallArgument.CreateStateArgument("state", GetStateType(), codeProviderArgument.StateField)
                              };

                var methodReference = GetAndCheckCodeProvidingMethodReferenceDefinition(methodInfo, methodArguments, destinationModule);

                if (methodReference.ContainsGenericParameter)
                {
                    var genericArgumentTypes = GetCodeProvidingMethodGenericArgumentTypes(codeProviderArgument);
                    var genericMethod = new GenericInstanceMethod(methodReference);
                    genericMethod.GenericArguments.AddRange(genericArgumentTypes);
                    methodReference = genericMethod;
                }

                CheckCodeProvidingMethodArguments(methodArguments);
                returnCallInfo.Add(new CodeProviderInjectionInfo(methodReference, methodArguments));
            }

            return returnCallInfo;
        }

        public abstract MethodInfo GetCodeProvidingMethod(MethodCodeInjectingCodeProviderArgument codeProviderArgument);

        public List<MethodDefinition> GetCodeProvidingMethoDefinition(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            var method = codeProviderArgument.Method;
            var methodName = method.Name;
            var methodBaseType = method.DeclaringComponent.Name;
            var methodCall = string.Empty;

            var handlers = from t in AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location).
                             CustomAttributes.AsQueryable()
                           where t.AttributeType.FullName == typeof(ExceptionHandlerAttribute).FullName
                           select t;

            var matchHandlers = from t in handlers
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(methodBaseType, ".", methodName)))
                                select t;

            var returnMethods = new List<MethodDefinition>();

            foreach (var item in matchHandlers)
            {
                methodCall = item.ConstructorArguments.Last().Value.ToString();
                var type = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location).MainModule.Types.First(x => x.Name == "MethodInjectionCodeProvider");
                returnMethods.Add(type.Methods.First(x => x.Name == methodCall));
            }

            return returnMethods;
        }

        public abstract CodeProviderCallArgument[] GetCodeProvidingMethodArguments(MethodCodeInjectingCodeProviderArgument codeProviderArgument);

        public virtual TypeReference[] GetCodeProvidingMethodGenericArgumentTypes(MethodCodeInjectingCodeProviderArgument codeProviderArgument)
        {
            throw new NotImplementedException($"For usage of generic code providing method {nameof(GetCodeProvidingMethodGenericArgumentTypes)} must be properly implemented.");
        }

        public MethodReference GetAndCheckCodeProvidingMethodReference([NotNull]MethodInfo method, [NotNull]CodeProviderCallArgument[] codeProvidingMethodArguments, [NotNull]ModuleDefinition destinationModule)
        {
            var methodDeclaringType = method.DeclaringType;

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be static to be injected.");
            }

            if (!method.IsPublic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be public to be injected.");
            }

            if (method.ReturnParameter.ParameterType != typeof(void))
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must have Void return type to be injected. Now it returns '{method.ReturnParameter.ParameterType.FullName}'.");
            }

            var methodParams = method.GetParameters();

            if (methodParams.Length != codeProvidingMethodArguments.Length)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' is configured to contain {codeProvidingMethodArguments.Length} parameters but it has {methodParams.Length} parameters.");
            }

            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].Name != codeProvidingMethodArguments[i].Name)
                {
                    throw new InvalidOperationException($"Name of {i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' is configured to be '{codeProvidingMethodArguments[i].Name}' but it is '{methodParams[i].Name}'.");
                }

                if (methodParams[i].ParameterType != codeProvidingMethodArguments[i].ClrType)
                {
                    if (codeProvidingMethodArguments[i].ClrType != null)
                    {
                        throw new InvalidOperationException($"Type of {i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be '{codeProvidingMethodArguments[i].ClrType.FullName}' but is '{methodParams[i].ParameterType.FullName}'.");
                    }

                    if (!methodParams[i].ParameterType.IsGenericParameter)
                    {
                        throw new InvalidOperationException($"Parameter '{codeProvidingMethodArguments[i].Name}' of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be generic.");
                    }
                }
            }

            return destinationModule.Import(method);
        }

        public MethodReference GetAndCheckCodeProvidingMethodReferenceDefinition([NotNull]MethodDefinition method, [NotNull]CodeProviderCallArgument[] codeProvidingMethodArguments, [NotNull]ModuleDefinition destinationModule)
        {
            var methodDeclaringType = method.DeclaringType;

            if (!method.IsStatic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be static to be injected.");
            }

            if (!method.IsPublic)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must be public to be injected.");
            }

            if (method.ReturnType.FullName != typeof(void).FullName)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' must have Void return type to be injected. Now it returns '{method.ReturnType.GetType().FullName}'.");
            }

            var methodParams = method.Parameters;

            if (methodParams.Count != codeProvidingMethodArguments.Length)
            {
                throw new InvalidOperationException($"Method '{method.Name}' on type '{methodDeclaringType.FullName}' is configured to contain {codeProvidingMethodArguments.Length} parameters but it has {methodParams.Count} parameters.");
            }

            for (int i = 0; i < methodParams.Count; i++)
            {
                if (methodParams[i].Name != codeProvidingMethodArguments[i].Name)
                {
                    throw new InvalidOperationException($"Name of {i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' is configured to be '{codeProvidingMethodArguments[i].Name}' but it is '{methodParams[i].Name}'.");
                }

                if (methodParams[i].ParameterType.FullName != codeProvidingMethodArguments[i].ClrType.FullName)
                {
                    /*
                    if (codeProvidingMethodArguments[i].ClrType != null)
                    {
                        throw new InvalidOperationException($"Type of {i}. parameter of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be '{codeProvidingMethodArguments[i].ClrType.FullName}' but is '{methodParams[i].ParameterType.FullName}'.");
                    }

                    if (!methodParams[i].ParameterType.IsGenericParameter)
                    {
                        throw new InvalidOperationException($"Parameter '{codeProvidingMethodArguments[i].Name}' of method '{method.Name}' on type '{methodDeclaringType.FullName}' should be generic.");
                    } 
                    */
                }
            }

            return destinationModule.Import(method);
        }
    }
}
