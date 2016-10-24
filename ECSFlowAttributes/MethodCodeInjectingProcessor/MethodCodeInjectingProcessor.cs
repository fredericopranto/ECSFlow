using System;
using ExtensibleILRewriter;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.Processors.Methods;
using ExtensibleILRewriter.Processors.Parameters;

namespace ECSFlowAttributes.MethodCodeInjectingProcessor
{
    public class MethodCodeInjectingProcessor : MethodCodeInjectingProcessor<MethodCodeInjectingProcessorConfiguration>
    {
        public MethodCodeInjectingProcessor([NotNull]MethodCodeInjectingProcessorConfiguration configuration, [NotNull]ILogger logger)
            : base(configuration, logger)
        {
        }
        
        protected override MethodInjectionPlace GetInjectionPlace(MethodProcessableComponent method)
        {
            var name = method.Name;
            if (true)
            {
                return MethodInjectionPlace.Begining;
            }
            else if (name.StartsWith(MethodInjectionCodeProvider.InjectOnExitPrefix))
            {
                return MethodInjectionPlace.Exit;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        

        protected override string GetStateInstanceName(MethodProcessableComponent method)
        {
            return "TestsInjectionState";
        }
    }
}
