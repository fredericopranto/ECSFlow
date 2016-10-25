using ExtensibleILRewriter.CodeInjection;
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace ExtensibleILRewriter.Tests.AddAttributeProcessor
{
    public class InjectedAttributeProvider : AttributeProvider
    {
        public override IEnumerable<Type> GetAttributeMapping(IProcessableComponent component)
        {
            throw new NotImplementedException();
        }

        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(IProcessableComponent component, Type attr)
        {
            var typeDefinition = component.Type == ProcessableComponentType.Type ? ((TypeDefinition)component.UnderlyingComponent) : null;

            if (component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute1Prefix))
            {
                return new AttributeProviderAttributeArgument[]
                {
                    AttributeProviderAttributeArgument.CreateParameterArgument("component", component.Type),
                    AttributeProviderAttributeArgument.CreateParameterArgument("type", typeDefinition),
                    AttributeProviderAttributeArgument.CreateParameterArgument("nameHash", component.Name.GetHashCode()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("name", component.Name)
                };
            }
            else if (component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute2Prefix))
            {
                return new AttributeProviderAttributeArgument[]
                {
                    AttributeProviderAttributeArgument.CreateParameterArgument("component", new Enum[] { component.Type, component.Type, component.Type }),
                    AttributeProviderAttributeArgument.CreateParameterArgument("type", new TypeReference[] { typeDefinition, typeDefinition, typeDefinition }),
                    AttributeProviderAttributeArgument.CreateParameterArgument("nameHash", new int[] { component.Name.GetHashCode(), component.Name.GetHashCode(), component.Name.GetHashCode() }),
                    AttributeProviderAttributeArgument.CreateParameterArgument("name", new string[] { component.Name, component.Name, component.Name })
                };
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected override Type GetAttributeType(IProcessableComponent component, Type attr)
        {
            if (component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute1Prefix))
            {
                return typeof(Injected1Attribute);
            }
            else if (component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute2Prefix))
            {
                return typeof(Injected2Attribute);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected override bool ShouldBeInjected(IProcessableComponent component, Type att)
        {
            return component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute1Prefix) || component.Name.StartsWith(AddAttributeProcessorTests.InjectAttribute2Prefix);
        }
    }
}
