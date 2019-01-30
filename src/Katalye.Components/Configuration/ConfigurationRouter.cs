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
                if (methodInfo.IsSpecialName && methodInfo.Name.StartsWith("get_"))
                {
                    var propertyName = methodInfo.Name.Substring("get_".Length);
                    var propertyMemberInfo = type.GetProperty(propertyName);
                    var member = propertyMemberInfo?.GetCustomAttribute<ConfigurationMemberAttribute>();
                    var shouldInterceptMethod = member != null;
                    return shouldInterceptMethod;
                }

                return false;
            }
        }
    }
}