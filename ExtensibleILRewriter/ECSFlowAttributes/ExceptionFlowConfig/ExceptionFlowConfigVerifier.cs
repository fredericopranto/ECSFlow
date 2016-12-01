using ECSFlowAttributes;

namespace ExtensibleILRewriter.ECSFlowAttributes.ExceptionFlowConfig
{
    public class ExceptionFlowConfigVerifier
    {
        public static void Verify(IECSFlowAttribute attribute)
        {
            if (attribute.GetType() == typeof(ExceptionChannelAttribute))
            {
            }
            else if (attribute.GetType() == typeof(ExceptionHandlerAttribute))
            {
            }
            else if (attribute.GetType() == typeof(ExceptionRaiseSiteAttribute))
            {
            }
            else if (attribute.GetType() == typeof(ExceptionInterfaceAttribute))
            {
            }
        }
    }
}
