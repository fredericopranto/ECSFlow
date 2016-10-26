using System;
using Mono.Cecil.Cil;

namespace ExtensibleILRewriter.CodeInjection
{
    public class WeavingException : Exception
    {
        public WeavingException(string message)
            : base(message)
        {
        }

        public SequencePoint SequencePoint { get; set; }
    }
}