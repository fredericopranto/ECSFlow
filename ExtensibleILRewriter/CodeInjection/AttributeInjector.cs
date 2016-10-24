using ExtensibleILRewriter.Extensions;
using ExtensibleILRewriter.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensibleILRewriter.CodeInjection
{
    public class AttributeInjector
    {
        private readonly AttributeProvider attributeProvider;

        public AttributeInjector(AttributeProvider attributeProvider)
        {
            this.attributeProvider = attributeProvider;
        }

        public void AddAttributeFromProvider(IProcessableComponent component, ILogger logger)
        {
            foreach (var customAttribute in attributeProvider.GetAttributeMapping(component))
            {
                var attributeInfo = attributeProvider.GetAttributeInfo(component, customAttribute);

                if (!attributeInfo.ShouldBeAttributeInjected)
                {
                    return;
                }

                logger.Notice($"Injecting attribute to {component.FullName}.");

                component.CustomAttributes.Add(attributeInfo.CustomAttribute);
            }
        }
    }
}