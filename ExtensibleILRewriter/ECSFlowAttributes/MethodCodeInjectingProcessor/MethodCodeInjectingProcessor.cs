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
            return MethodInjectionPlace.InCatchBlock;
        }

        protected override string GetStateInstanceName(MethodProcessableComponent method)
        {
            return "ECSFlowInjectionState";
        }
    }
}
