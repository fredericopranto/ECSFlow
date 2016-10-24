using Mono.Cecil;

namespace ExtensibleILRewriter.Processors.Methods
{
    public struct MethodCodeInjectingCodeProviderArgument
    {
        private FieldDefinition stateField;

        public MethodCodeInjectingCodeProviderArgument(MethodProcessableComponent method, FieldDefinition stateField)
        {
            Method = method;
            this.stateField = stateField;
        }

        public MethodProcessableComponent Method { get; }

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
    }
}
