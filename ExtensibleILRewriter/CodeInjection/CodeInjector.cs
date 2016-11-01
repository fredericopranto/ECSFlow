using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Methods;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Linq;

namespace ExtensibleILRewriter.CodeInjection
{
    public class CodeInjector<CodeProviderArgumentType>
    {
        private readonly ModuleDefinition module;
        private readonly CodeProvider<CodeProviderArgumentType> codeProvider;
        private readonly Collection<Instruction> newInstructions = new Collection<Instruction>();

        public CodeInjector(ModuleDefinition module, CodeProvider<CodeProviderArgumentType> codeProvider)
        {
            this.module = module;
            this.codeProvider = codeProvider;
        }

        public delegate void CustomInstructionsInjection(MethodBody body, Collection<Instruction> newInstructions, MethodDefinition methodCall);

        public bool ShouldBeCallInjected(CodeProviderArgumentType codeProviderArgument)
        {
            return codeProvider.ShouldBeInjected(codeProviderArgument);
        }

        public void InjectAtBegining(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace)
        {
            Inject(
                method,
                codeProviderArgument,
                logger, injectionPlace,
                (body, newInstructions, call) => body.AddInstructionsToBegining(newInstructions));
        }

        public void InjectBeforeExit(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace)
        {
            Inject(
                method,
                codeProviderArgument,
                logger, injectionPlace,
                (body, newInstruction, call) => body.AddInstructionsBeforeExit(newInstructions));
        }

        public void InjectInCatchBlock(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace)
        {
            Inject(
                method,
                codeProviderArgument,
                logger, injectionPlace,
                (body, newInstruction, call) => body.AddInstructionsInCatchBlock(newInstruction, call));
        }

        public void InjectInFinallyBlock(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger)
        {
            throw new InvalidOperationException($"Under construction injection place specified.");
        }

        public void Inject(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace, CustomInstructionsInjection injectNewInstructions)
        {
            var callInfoCollection = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!method.HasBody)
            {
                throw new ArgumentException("Method does not contain body.");
            }

            logger.Notice($"Injecting method call before exit of method '{method.FullName}'.");

            newInstructions.Clear();

            if (injectionPlace == MethodInjectionPlace.InCatchBlock)
            {
                GenerateInstructionsForInjectedCallInTryCatchBlock(method, newInstructions, callInfoCollection[0].MethodReferenceToBeCalled, callInfoCollection[0].CallArguments);
            }
            else
            {
                GenerateInstructionsForInjectedCall(newInstructions, callInfoCollection[0].MethodReferenceToBeCalled, callInfoCollection[0].CallArguments);
            }

            foreach (var callInfo in callInfoCollection)
            {
                injectNewInstructions(method.Body, newInstructions, callInfo.MethodReferenceToBeCalled.Resolve());
            }
        }

        private static void GenerateInstructionsForInjectedCall(Collection<Instruction> instructions, MethodReference methodCall, CodeProviderCallArgument[] arguments)
        {
            foreach (var arg in arguments)
            {
                instructions.Add(arg.GenerateLoadInstruction());
            }

            instructions.Add(Instruction.Create(OpCodes.Call, methodCall));
        }

        private static void GenerateInstructionsForInjectedCallInTryCatchBlock(MethodDefinition method, Collection<Instruction> instructions, MethodReference methodCall, CodeProviderCallArgument[] arguments)
        {
            foreach (var arg in arguments)
            {
                instructions.Add(arg.GenerateLoadInstruction());
            }

            instructions.Add(Instruction.Create(OpCodes.Call, method.Module.Import(typeof(Console).GetMethod("WriteLine", new[] { typeof(object) }))));
        }

        private static Instruction GetMethodBodyFirstInstruction(MethodDefinition method)
        {
            if (method.IsConstructor)
            {
                return method.Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;
            }

            return method.Body.Instructions.First();
        }
    }
}