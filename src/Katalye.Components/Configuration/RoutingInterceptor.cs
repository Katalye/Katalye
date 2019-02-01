using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Katalye.Components.Configuration.Providers;
using Katalye.Components.Configuration.ValueParsers;
using NLog;

namespace Katalye.Components.Configuration
{
    public interface IRoutingInterceptor : IInterceptor
    {
    }

    public delegate IValueParser CreateValueParser(Type type);

    public class RoutingInterceptor : IRoutingInterceptor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CreateValueParser _createValueParser;
        private readonly IList<IConfigurationProvider> _providers;

        public RoutingInterceptor(IList<IConfigurationProvider> providers, CreateValueParser createValueParser)
        {
            _createValueParser = createValueParser;
            _providers = providers
                         .OrderBy(x => x.Priority)
                         .ToList();
        }

        public void Intercept(IInvocation invocation)
        {
            var configuration = ConfigurationMemberHelper.GetConfigurationMemberAttributeFromMethodInfo(invocation.Method)
                                ?? throw new Exception($"No {nameof(ConfigurationMemberAttribute)} could be found, "
                                                       + "this should be impossible at this point");

            foreach (var provider in _providers)
            {
                var (success, value) = provider.TryGet(configuration.Path, configuration.DefaultValue);
                if (success)
                {
                    var parsedValue = ParseResult(invocation.Method.ReturnType, value, configuration.Path);
                    invocation.ReturnValue = parsedValue;
                    return;
                }
            }
        }

        private object ParseResult(Type type, string value, string path)
        {
            object result = null;
            try
            {
                var parser = _createValueParser.Invoke(type);
                if (parser != null)
                {
                    result = parser.Parse(value);
                }
                else
                {
                    Logger.Error($"For configuration path [{path}] no parser for type {type.FullName} exists, returning a default value.");
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, $"For configuration path [{path}] failed to parse value [{value}] to type [{type.FullName}], "
                               + "a default value will be used.");
            }

            return result ?? GetDefaultValue(type);
        }

        private object GetDefaultValue(Type t)
        {
            // https://stackoverflow.com/a/2490274/2001966
            return t.IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }
}