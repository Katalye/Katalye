using System.Reflection;

namespace Katalye.Components.Configuration
{
    public static class ConfigurationMemberHelper
    {
        public static ConfigurationMemberAttribute GetConfigurationMemberAttributeFromMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.IsSpecialName && methodInfo.Name.StartsWith("get_") && methodInfo.DeclaringType != null)
            {
                var propertyName = methodInfo.Name.Substring("get_".Length);
                var propertyMemberInfo = methodInfo.DeclaringType.GetProperty(propertyName);
                var member = propertyMemberInfo?.GetCustomAttribute<ConfigurationMemberAttribute>();

                return member;
            }

            return null;
        }
    }
}