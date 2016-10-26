using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace ExtensibleILRewriter.CodeInjection
{
    public class ReturnFixer
    {
        private MethodDefinition method;
        private Instruction nopForHandleEnd;
        private Collection<Instruction> instructions;
        private Instruction nopBeforeReturn;
        private Instruction sealBranchesNop;
        private VariableDefinition returnVariable;

        public MethodDefinition Method
        {
            get
            {
                return method;
            }

            set
            {
                method = value;
            }
        }

        public Instruction NopForHandleEnd
        {
            get
            {
                return nopForHandleEnd;
            }

            set
            {
                nopForHandleEnd = value;
            }
        }

        public Instruction NopBeforeReturn
        {
            get
            {
                return nopBeforeReturn;
            }

            set
            {
                nopBeforeReturn = value;
            }
        }

        public Collection<Instruction> Instructions
        {
            get
            {
                return instructions;
            }

            set
            {
                instructions = value;
            }
        }

        public Instruction SealBranchesNop
        {
            get
            {
                return sealBranchesNop;
            }

            set
            {
                sealBranchesNop = value;
            }
        }

        public VariableDefinition ReturnVariable
        {
            get
            {
                return returnVariable;
            }

            set
            {
                returnVariable = value;
            }
        }

        public void MakeLastStatementReturn()
        {
            Instructions = Method.Body.Instructions;
            FixHangingHandlerEnd();

            SealBranchesNop = Instruction.Create(OpCodes.Nop);
            Instructions.Add(SealBranchesNop);

            NopBeforeReturn = Instruction.Create(OpCodes.Nop);

            if (IsMethodReturnValue())
            {
                ReturnVariable = new VariableDefinition(Method.MethodReturnType.ReturnType);
                Method.Body.Variables.Add(ReturnVariable);
            }

            for (var index = 0; index < Instructions.Count; index++)
            {
                var operand = Instructions[index].Operand as Instruction;
                if (operand != null)
                {
                    if (operand.OpCode == OpCodes.Ret)
                    {
                        if (IsMethodReturnValue())
                        {
                            // The C# compiler never jumps directly to a ret
                            // when returning a value from the method. But other Fody
                            // modules and other compilers might. So store the value here.
                            Instructions.Insert(index, Instruction.Create(OpCodes.Stloc, ReturnVariable));
                            Instructions.Insert(index, Instruction.Create(OpCodes.Dup));
                            index += 2;
                        }

                        Instructions[index].Operand = SealBranchesNop;
                    }
                }
            }

            if (!IsMethodReturnValue())
            {
                WithNoReturn();
                return;
            }

            WithReturnValue();
        }

        public bool IsMethodReturnValue()
        {
            return Method.MethodReturnType.ReturnType.Name != "Void";
        }

        public void FixHangingHandlerEnd()
        {
            if (Method.Body.ExceptionHandlers.Count == 0)
            {
                return;
            }

            NopForHandleEnd = Instruction.Create(OpCodes.Nop);
            Method.Body.Instructions.Add(NopForHandleEnd);
            foreach (var handler in Method.Body.ExceptionHandlers)
            {
                if (handler.HandlerStart != null && handler.HandlerEnd == null)
                {
                    handler.HandlerEnd = NopForHandleEnd;
                }
            }
        }

        private void WithReturnValue()
        {
            for (var index = 0; index < Instructions.Count; index++)
            {
                var instruction = Instructions[index];
                if (instruction.OpCode == OpCodes.Ret)
                {
                    Instructions.Insert(index, Instruction.Create(OpCodes.Stloc, ReturnVariable));
                    instruction.OpCode = OpCodes.Br;
                    instruction.Operand = SealBranchesNop;
                    index++;
                }
            }

            Instructions.Add(NopBeforeReturn);
            Instructions.Add(Instruction.Create(OpCodes.Ldloc, ReturnVariable));
            Instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void WithNoReturn()
        {
            foreach (var instruction in Instructions)
            {
                if (instruction.OpCode == OpCodes.Ret)
                {
                    instruction.OpCode = OpCodes.Br;
                    instruction.Operand = SealBranchesNop;
                }
            }

            Instructions.Add(NopBeforeReturn);
            Instructions.Add(Instruction.Create(OpCodes.Ret));
        }
    }
}