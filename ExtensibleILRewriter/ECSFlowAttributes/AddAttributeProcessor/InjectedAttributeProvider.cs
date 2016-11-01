using ExtensibleILRewriter;
using ExtensibleILRewriter.CodeInjection;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECSFlowAttributes.AddAttributeProcessor
{
    public class InjectedAttributeProvider : AttributeProvider
    {
        public override IEnumerable<Type> GetAttributeMapping(IProcessableComponent component)
        {
            var CurrentAttributeTypes = new List<Type>();

            var rsites = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                         where t.AttributeType.FullName == typeof(ExceptionRaiseSiteAttribute).FullName
                         select t;

            var matchRaiseSite = from t in rsites
                                 where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(component.DeclaringComponent.Name, ".", component.Name)))
                                 select t;

            var raiseSiteArguments = from t in matchRaiseSite
                                     select t.ConstructorArguments.ToArray();

            foreach (var raisesite in raiseSiteArguments)
            {
                var raisesitename = raisesite[0].Value;

                CurrentAttributeTypes.Add(typeof(ExceptionRaiseSiteAttribute));

                var channels = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                               where t.AttributeType.FullName == typeof(ExceptionChannelAttribute).FullName
                               select t;

                var channelsByRaiseSite = from t in channels
                                          where t.ConstructorArguments.Any(v => v.Value.ToString().Equals(raisesitename))
                                          select t;

                foreach (var channel in channelsByRaiseSite)
                {
                    CurrentAttributeTypes.Add(typeof(ExceptionChannelAttribute));
                }
            }

            var handlers = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                           where t.AttributeType.FullName == typeof(ExceptionHandlerAttribute).FullName
                           select t;

            var matchHandlers = from t in handlers
                                where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(component.DeclaringComponent.Name, ".", component.Name)))
                                select t;

            foreach (var item in matchHandlers)
            {
                CurrentAttributeTypes.Add(typeof(ExceptionHandlerAttribute));
            }

            return CurrentAttributeTypes;
        }

        protected override AttributeProviderAttributeArgument[] GetAttributeArguments(IProcessableComponent component, Type attr)
        {
            var typeDefinition = component.Type == ProcessableComponentType.Type ? ((TypeDefinition)component.UnderlyingComponent) : null;

            if (attr.Name.Equals(typeof(ExceptionRaiseSiteAttribute).Name))
            {
                return new AttributeProviderAttributeArgument[]
                 {
                    AttributeProviderAttributeArgument.CreateParameterArgument("name", CurrentAttributeArguments[0].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("target",  CurrentAttributeArguments[1].ToString())
                 };
            }
            else if (attr.Name.Equals(typeof(ExceptionChannelAttribute).Name))
            {
                return new AttributeProviderAttributeArgument[]
                 {
                    AttributeProviderAttributeArgument.CreateParameterArgument("name", CurrentAttributeArguments[0].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("exception",  CurrentAttributeArguments[1].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("raiseSite",  CurrentAttributeArguments[2].ToString())
                 };
            }
            else if (attr.Name.Equals(typeof(ExceptionHandlerAttribute).Name))
            {
                return new AttributeProviderAttributeArgument[]
                 {
                    AttributeProviderAttributeArgument.CreateParameterArgument("channel", CurrentAttributeArguments[0].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("target",  CurrentAttributeArguments[1].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("exception",  CurrentAttributeArguments[2].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("methodName",  CurrentAttributeArguments[3].ToString()),
                 };
            }
            else if (attr.Name.Equals(typeof(ExceptionInterfaceAttribute).Name))
            {
                return new AttributeProviderAttributeArgument[]
                 {
                    AttributeProviderAttributeArgument.CreateParameterArgument("name", CurrentAttributeArguments[0].ToString()),
                    AttributeProviderAttributeArgument.CreateParameterArgument("target",  CurrentAttributeArguments[1].ToString())
                 };
            }
            else
            {
                throw new ArgumentException();
            }
        }

        protected override Type GetAttributeType(IProcessableComponent component, Type attr)
        {
            if (attr.Name.Equals(typeof(ExceptionRaiseSiteAttribute).Name))
            {
                return typeof(ExceptionRaiseSiteAttribute);
            }
            else if (attr.Name.Equals(typeof(ExceptionChannelAttribute).Name))
            {
                return typeof(ExceptionChannelAttribute);
            }
            else if (attr.Name.Equals(typeof(ExceptionHandlerAttribute).Name))
            {
                return typeof(ExceptionHandlerAttribute);
            }
            else if (attr.Name.Equals(typeof(ExceptionInterfaceAttribute).Name))
            {
                return typeof(ExceptionInterfaceAttribute);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        protected override bool ShouldBeInjected(IProcessableComponent component, Type attr)
        {
            bool ShouldBeInjected = false;
            CurrentAttributeArguments = new ArrayList();

            if (attr.Name.Equals(typeof(ExceptionRaiseSiteAttribute).Name))
            {
                var rsites = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                             where t.AttributeType.FullName == typeof(ExceptionRaiseSiteAttribute).FullName
                             select t;

                var matchRaiseSite = from t in rsites
                                     where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(component.DeclaringComponent.Name, ".", component.Name)))
                                     select t;

                var raiseSiteArguments = from t in matchRaiseSite
                                         select t.ConstructorArguments.ToArray();

                foreach (var item in raiseSiteArguments)
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        CurrentAttributeArguments.Add(item[i].Value);
                    }

                    ShouldBeInjected = true;
                }
            }
            else if (attr.Name.Equals(typeof(ExceptionChannelAttribute).Name))
            {
                var rsites = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                             where t.AttributeType.FullName == typeof(ExceptionRaiseSiteAttribute).FullName
                             select t;

                var matchRaiseSite = from t in rsites
                                     where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(component.DeclaringComponent.Name, ".", component.Name)))
                                     select t;

                var raiseSiteArguments = from t in matchRaiseSite
                                         select t.ConstructorArguments.ToArray();

                foreach (var raisesite in raiseSiteArguments)
                {
                    var raisesitename = raisesite[0].Value;

                    var channels = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                                   where t.AttributeType.FullName == typeof(ExceptionChannelAttribute).FullName
                                   select t;

                    var channelsByRaiseSite = from t in channels
                                              where t.ConstructorArguments.Any(v => v.Value.ToString().Equals(raisesitename))
                                              select t;

                    var channelArguments = from t in channelsByRaiseSite
                                           select t.ConstructorArguments.ToArray();

                    foreach (var item in channelArguments)
                    {
                        for (int i = 0; i < item.Length; i++)
                        {
                            CurrentAttributeArguments.Add(item[i].Value);
                        }

                        ShouldBeInjected = true;
                    }
                }
            }
            else if (attr.Name.Equals(typeof(ExceptionHandlerAttribute).Name))
            {
                var handlers = from t in Assembly.GetExecutingAssembly().CustomAttributes.AsQueryable()
                               where t.AttributeType.FullName == typeof(ExceptionHandlerAttribute).FullName
                               select t;

                var matchHandlers = from t in handlers
                                    where t.ConstructorArguments.Any(item => item.Value.ToString().Equals(String.Concat(component.DeclaringComponent.Name, ".", component.Name)))
                                    select t;

                var handlersArguments = from t in matchHandlers
                                        select t.ConstructorArguments.ToArray();

                foreach (var item in handlersArguments)
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        CurrentAttributeArguments.Add(item[i].Value);
                    }

                    ShouldBeInjected = true;
                }
            }
            else if (attr.Name.Equals(typeof(ExceptionInterfaceAttribute).Name))
            {
                throw new ArgumentException();
            }
            else
            {
                throw new ArgumentException();
            }

            return ShouldBeInjected;
        }
    }
}
