using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Katalye.Components.Configuration.Providers;
using NLog;

namespace Katalye.Components.Configuration
{
    public interface IRoutingInterceptor : IInterceptor
    {
    }

    public class RoutingInterceptor : IRoutingInterceptor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IList<IConfigurationProvider> _providers;

        public RoutingInterceptor(IList<IConfigurationProvider> providers)
        {
            _providers = providers
                         .OrderByDescending(x => x.Priority)
                         .ToList();
        }

        public void Intercept(IInvocation invocation)
        {
            var propertyName = invocation.Method.Name.Substring("get_".Length);
            var propertyMemberInfo = invocation.Method.DeclaringType?.GetProperty(propertyName);
            var configuration = propertyMemberInfo?.GetCustomAttribute<ConfigurationMemberAttribute>()
                                ?? throw new Exception($"No {nameof(ConfigurationMemberAttribute)} could be found, "
                                                       + "this should be impossible at this point");

            foreach (var provider in _providers)
            {
                var result = provider.TryGet(configuration.Path, configuration.DefaultValue);
                if (result.Success)
                {
                    var parsedValue = ParseResult(invocation.Method.ReturnType, result.Value);
                    invocation.ReturnValue = parsedValue;
                }
            }
        }

        public object ParseResult(Type type, string value)
        {
            object result = null;
            try
            {
                if (type == typeof(string))
                {
                    result = value;
                }
                else if (type == typeof(int))
                {
                    result = int.Parse(value);
                }
                else if (type == typeof(long))
                {
                    result = long.Parse(value);
                }
                else if (type == typeof(bool))
                {
                    result = bool.Parse(value);
                }
                else if (type == typeof(decimal))
                {
                    result = decimal.Parse(value);
                }
                else if (type == typeof(Uri))
                {
                    return new Uri(value);
                }
                else
                {
                    Logger.Error($"No parser for type {type.FullName} exists, returning a default value.");
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, $"Failed to parse value [{value}] to type [{type.FullName}], "
                               + "a default value will be used.");
            }

            if (result == null)
            {
                result = GetDefaultValue(type);
            }

            return result;
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