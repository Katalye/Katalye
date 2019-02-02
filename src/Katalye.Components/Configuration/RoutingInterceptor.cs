using System;
using Castle.DynamicProxy;
using Katalye.Components.Configuration.ValueParsers;

namespace Katalye.Components.Configuration
{
    public interface IRoutingInterceptor : IInterceptor
    {
    }

    public delegate IValueParser CreateValueParser(Type type);

    public class RoutingInterceptor : IRoutingInterceptor
    {
        private readonly IConfigurationRenderer _configurationRenderer;

        public RoutingInterceptor(IConfigurationRenderer configurationRenderer)
        {
            _configurationRenderer = configurationRenderer;
        }

        public void Intercept(IInvocation invocation)
        {
            var configuration = ConfigurationMemberHelper.GetConfigurationMemberAttributeFromMethodInfo(invocation.Method)
                                ?? throw new Exception($"No {nameof(ConfigurationMemberAttribute)} could be found, "
                                                       + "this should be impossible at this point");

            var result = _configurationRenderer.RenderSetting(configuration.Path, invocation.Method.ReturnType);
            invocation.ReturnValue = result;
        }
    }
}