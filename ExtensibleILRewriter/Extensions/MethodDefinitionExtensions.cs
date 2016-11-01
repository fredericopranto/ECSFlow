using ExtensibleILRewriter.CodeInjection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensibleILRewriter.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static bool CouldBeStatic(this MethodDefinition method)
        {
            if (!method.HasBody)
            {
                return false;
            }

            if (method.IsStatic)
            {
                return true;
            }

            return !AccessesThis(method.Body);
        }

        public static void AddInstructionToBegining(this MethodBody body, Instruction newInstruction)
        {
            body.SimplifyMacros();

            body.Instructions.Insert(0, newInstruction);

            body.OptimizeMacros();
        }

        public static void AddInstructionsToBegining(this MethodBody body, Collection<Instruction> newInstructions, bool repairSequencePoints = true)
        {
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();
            instructions.AddRange(newInstructions);
            instructions.AddRange(oldInstructions);

            if (repairSequencePoints)
            {
                var sequencePoint = instructions.FirstOrDefault(i => i.SequencePoint != null && i.SequencePoint.Document != null)?.SequencePoint;
                if (sequencePoint != null)
                {
                    var firstInstruction = instructions[0];
                    firstInstruction.SequencePoint = new SequencePoint(sequencePoint.Document);
                    firstInstruction.SequencePoint.StartLine = sequencePoint.StartLine;
                    firstInstruction.SequencePoint.EndLine = sequencePoint.EndLine;
                    firstInstruction.SequencePoint.StartColumn = sequencePoint.StartColumn;
                    firstInstruction.SequencePoint.EndColumn = sequencePoint.EndColumn;
                }
            }

            body.OptimizeMacros();
        }

        public static void AddInstructionsBeforeExit(this MethodBody body, Collection<Instruction> newInstructions)
        {
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();

            var branchOperandReplacements = new Dictionary<Instruction, Instruction>();
            foreach (var ins in oldInstructions)
            {
                if (ins.OpCode.Code == Code.Ret)
                {
                    instructions.AddRange(newInstructions);
                    branchOperandReplacements.Add(ins, newInstructions[0]); // all branches should go to our new instruction instead of directly going to ret
                }

                instructions.Add(ins);
            }

            // repair branching to ret instructions, so the branching is made to begining of injected code
            foreach (var ins in instructions)
            {
                if (ins.OpCode.FlowControl == FlowControl.Branch || ins.OpCode.FlowControl == FlowControl.Cond_Branch)
                {
                    var branchOperand = (Instruction)ins.Operand;

                    Instruction newBranchOperand;
                    if (branchOperandReplacements.TryGetValue(branchOperand, out newBranchOperand))
                    {
                        ins.Operand = newBranchOperand;
                    }
                }
            }

            body.OptimizeMacros();
        }

        public static void AddInstructionsInCatchBlock(this MethodBody body, Collection<Instruction> newInstructions, MethodReference methodCall)
        {
            /*
            
            body.SimplifyMacros();

            var instructions = body.Instructions;
            var oldInstructions = instructions.ToArray();

            instructions.Clear();

            var branchOperandReplacements = new Dictionary<Instruction, Instruction>();
            foreach (var ins in oldInstructions)
            {
                if (ins.OpCode.Code == Code.Ret)
                {
                    instructions.AddRange(newInstructions);
                    branchOperandReplacements.Add(ins, newInstructions[0]); // all branches should go to our new instruction instead of directly going to ret
                }

                instructions.Add(ins);
            }

            // repair branching to ret instructions, so the branching is made to begining of injected code
            foreach (var ins in instructions)
            {
                if (ins.OpCode.FlowControl == FlowControl.Branch || ins.OpCode.FlowControl == FlowControl.Cond_Branch)
                {
                    var branchOperand = (Instruction)ins.Operand;

                    Instruction newBranchOperand;
                    if (branchOperandReplacements.TryGetValue(branchOperand, out newBranchOperand))
                    {
                        ins.Operand = newBranchOperand;
                    }
                }
            }

            // instructions.RemoveAt(14);
            // instructions.Add(body.GetILProcessor().Create(OpCodes.Leave));
            body.OptimizeMacros();

            */

            /*

            var il = body.GetILProcessor();

            var write = il.Create(OpCodes.Call, body.Method.Module.Import(methodCall));
            
            // var write = il.Create(OpCodes.Call, body.Method.Module.Import(typeof(Console).GetMethod("WriteLine", new[] { typeof(object) })));
            var ret = il.Create(OpCodes.Ret);
            var leave = il.Create(OpCodes.Leave, ret);

            il.InsertAfter(
                body.Instructions.Last(),
                write);

            il.InsertAfter(write, leave);
            il.InsertAfter(leave, ret);

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = body.Instructions.First(),
                TryEnd = write,
                HandlerStart = write,
                HandlerEnd = ret,
                CatchType = body.Method.Module.Import(typeof(Exception)),
            };

            body.ExceptionHandlers.Add(handler);
            */

            body.SimplifyMacros();

            var ilProcessor = body.GetILProcessor();

            var returnFixer = new ReturnFixer
            {
                Method = body.Method
            };
            returnFixer.MakeLastStatementReturn();

            // Create a basic Try/Cacth Block
            var tryBlockLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);
            var catchBlockLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);

            // Get the first instruction to surround the Try/Catch Block
            var methodBodyFirstInstruction = GetMethodBodyFirstInstruction(body);

            var catchBlockInstructions = GetCatchInstructions(catchBlockLeaveInstructions, body.Method.Module.Import(methodCall)).ToList();
            ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, tryBlockLeaveInstructions);
            ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, catchBlockInstructions);

            var ExType = Type.GetType("System." + methodCall.Name);

            if (ExType == null)
            {
                ExType = Type.GetType("System.IO." + methodCall.Name);
            }

            var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = body.Method.Module.Import(ExType),
                TryStart = methodBodyFirstInstruction,
                TryEnd = tryBlockLeaveInstructions.Next,
                HandlerStart = catchBlockInstructions.First(),
                HandlerEnd = catchBlockInstructions.Last().Next
            };

            body.ExceptionHandlers.Add(handler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catchBlockLeaveInstructions"></param>
        /// <returns></returns>
        public static IEnumerable<Instruction> GetCatchInstructions(Instruction catchBlockLeaveInstructions, MethodReference def)
        {
            yield return Instruction.Create(OpCodes.Call, def);
            yield return Instruction.Create(OpCodes.Nop);
            yield return catchBlockLeaveInstructions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Instruction GetMethodBodyFirstInstruction(MethodBody Body)
        {
            if (Body.Method.IsConstructor)
            {
                return Body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;
            }

            return Body.Instructions.First();
        }

        private static bool AccessesThis(MethodBody methodBody)
        {
            foreach (var ins in methodBody.Instructions)
            {
                if (ins.OpCode.Code == Code.Ldarg_0)
                {
                    return true;
                }

                if (ins.Operand as ParameterDefinition == methodBody.ThisParameter)
                {
                    return true;
                }
            }

            return false;
        }
    }
}