using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace Katalye.Components.Configuration
{
    public class ConfigurationRouter
    {
        private readonly IProxyGenerator _generator;
        private readonly IRoutingInterceptor _interceptor;

        public ConfigurationRouter(IProxyGenerator generator, IRoutingInterceptor interceptor)
        {
            _generator = generator;
            _interceptor = interceptor;
        }

        public IKatalyeConfiguration CreateConfiguration()
        {
            var options = new ProxyGenerationOptions(new ConfigurationRouterOptions());
            var configuration = _generator.CreateInterfaceProxyWithoutTarget<IKatalyeConfiguration>(options, _interceptor);

            return configuration;
        }

        private class ConfigurationRouterOptions : IProxyGenerationHook
        {
            public void MethodsInspected()
            {
            }

            public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
            {
                throw new NotImplementedException();
            }

            public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return ConfigurationMemberHelper.GetConfigurationMemberAttributeFromMethodInfo(methodInfo) != null;
            }
        }
    }
}