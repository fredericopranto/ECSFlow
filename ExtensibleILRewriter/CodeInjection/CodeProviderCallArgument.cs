using ExtensibleILRewriter.Processors.Parameters;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace ExtensibleILRewriter.CodeInjection
{
    public struct CodeProviderCallArgument
    {
        private ParameterDefinition parameterDefinition;
        private FieldDefinition stateField;
        private string text;

        private CodeProviderCallArgument([NotNull]string name, CodeProviderCallArgumentType type, [NotNull]Type clrType)
        {
            Name = name;
            Type = type;
            ClrType = clrType;

            parameterDefinition = null;
            stateField = null;
            text = null;
        }

        public string Name { get; }

        public CodeProviderCallArgumentType Type { get; }

        public Type ClrType { get; }

        public static CodeProviderCallArgument[] EmptyCollection { get; } = new CodeProviderCallArgument[0];

        public FieldDefinition StateField
        {
            get
            {
                return stateField;
            }

            set
            {
                stateField = value;
            }
        }

        public static CodeProviderCallArgument CreateParameterArgument([NotNull]string name, [NotNull]Type clrType, [NotNull]ParameterDefinition parameterDefinition)
        {
            return new CodeProviderCallArgument(name, CodeProviderCallArgumentType.ParameterDefinition, clrType) { parameterDefinition = parameterDefinition };
        }

        public static CodeProviderCallArgument CreateGenericParameterArgument([NotNull]string name, [NotNull]ParameterDefinition parameterDefinition)
        {
            return new CodeProviderCallArgument(name, CodeProviderCallArgumentType.ParameterDefinition, null) { parameterDefinition = parameterDefinition };
        }

        public static CodeProviderCallArgument CreateStateArgument([NotNull]string name, [NotNull]Type clrType, [NotNull]FieldDefinition fieldDefinition)
        {
            return new CodeProviderCallArgument(name, CodeProviderCallArgumentType.FieldDefinition, clrType) { stateField = fieldDefinition };
        }

        public static CodeProviderCallArgument CreateTextArgument([NotNull]string name, string value)
        {
            return new CodeProviderCallArgument(name, CodeProviderCallArgumentType.String, typeof(string)) { text = value };
        }

        public Instruction GenerateLoadInstruction()
        {
            switch (Type)
            {
                case CodeProviderCallArgumentType.ParameterDefinition:
                    if (parameterDefinition == null)
                    {
                        throw new InvalidOperationException("FieldDefinition value must be set before generating load instruction.");
                    }

                    return Instruction.Create(OpCodes.Ldarg, parameterDefinition);
                case CodeProviderCallArgumentType.FieldDefinition:
                    if (stateField == null)
                    {
                        throw new InvalidOperationException("ParameterDefinition value must be set before generating load instruction.");
                    }

                    return Instruction.Create(OpCodes.Ldsfld, stateField);
                case CodeProviderCallArgumentType.String:
                    if (text == null)
                    {
                        throw new InvalidOperationException("String value must be set before generating load instruction.");
                    }

                    return Instruction.Create(OpCodes.Ldstr, text);
                default:
                    throw new NotImplementedException($"Unknown {nameof(CodeProviderCallArgument)} type '{Type}'.");
            }
        }
    }
}