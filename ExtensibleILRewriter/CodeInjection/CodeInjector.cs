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

        public delegate void CustomInstructionsInjection(MethodBody body, Collection<Instruction> newInstructions);

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
                (body, newInstructions) => body.AddInstructionsToBegining(newInstructions));
        }

        public void InjectBeforeExit(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace)
        {
            Inject(
                method,
                codeProviderArgument,
                logger, injectionPlace,
                (body, newInstructions) => body.AddInstructionsBeforeExit(newInstructions));
        }

        public void InjectInCatchBlock(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace)
        {
            Inject(
                method,
                codeProviderArgument,
                logger, injectionPlace,
                (body, newInstructions) => body.AddInstructionsInCatchBlock(newInstructions, method));
        }

        public void InjectInFinallyBlock(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger)
        {
            throw new InvalidOperationException($"Under construction injection place specified.");
        }

        public void Inject(MethodDefinition method, MethodCodeInjectingCodeProviderArgument codeProviderArgument, ILogger logger, MethodInjectionPlace injectionPlace, CustomInstructionsInjection injectNewInstructions)
        {
            var callInfo = codeProvider.GetCallInfo(codeProviderArgument, method.Module);

            if (!method.HasBody)
            {
                throw new ArgumentException("Method does not contain body.");
            }

            logger.Notice($"Injecting method call before exit of method '{method.FullName}'.");

            newInstructions.Clear();

            if (injectionPlace == MethodInjectionPlace.InCatchBlock)
            {
                GenerateInstructionsForInjectedCallInTryCatchBlock(method, newInstructions, callInfo.MethodReferenceToBeCalled, callInfo.CallArguments);
            }
            else
            {
                GenerateInstructionsForInjectedCall(newInstructions, callInfo.MethodReferenceToBeCalled, callInfo.CallArguments);
            }

            injectNewInstructions(method.Body, newInstructions);
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

            instructions.Add(Instruction.Create(OpCodes.Call, methodCall));

            /*
            foreach (var arg in arguments)
            {
                instructions.Add(arg.GenerateLoadInstruction());
            }*/

            /*
            

            var returnFixer = new ReturnFixer
            {
                Method = method
            };
            returnFixer.MakeLastStatementReturn();

            // Create a basic Try/Cacth Block
            var tryBlockLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);
            var catchBlockLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);

            // Get the first instruction to surround the Try/Catch Block
            var methodBodyFirstInstruction = GetMethodBodyFirstInstruction(method);

            instructions.Add(Instruction.Create(OpCodes.Call, methodCall));
            var catchBlockInstructions = instructions;
            var ilProcessor = method.Body.GetILProcessor();
            ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, tryBlockLeaveInstructions);
            ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, catchBlockInstructions);
            
            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = method.Module.Import(typeof(System.Exception)),
                TryStart = methodBodyFirstInstruction,
                TryEnd = tryBlockLeaveInstructions.Previous,
                HandlerStart = catchBlockInstructions.First(),
                HandlerEnd = catchBlockInstructions.Last().Next
            };

            method.Body.ExceptionHandlers.Add(handler);
            */

            /*
            var il = method.Body.GetILProcessor();

            var write = il.Create(OpCodes.Call, methodCall);
            var ret = il.Create(OpCodes.Ret);
            var leave = il.Create(OpCodes.Leave, ret);

            il.InsertAfter(method.Body.Instructions.Last(), write);

            il.InsertAfter(write, leave);
            il.InsertAfter(leave, ret);

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = method.Body.Instructions.First(),
                TryEnd = write,
                HandlerStart = write,
                HandlerEnd = ret,
                CatchType = method.Module.Import(typeof(Exception)),
            };

            method.Body.ExceptionHandlers.Add(handler);
            */
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