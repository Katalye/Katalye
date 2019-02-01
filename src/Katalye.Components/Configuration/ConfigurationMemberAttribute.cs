using System;

namespace Katalye.Components.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationMemberAttribute : Attribute
    {
        public string Path { get; set; }

        public string DefaultValue { get; set; } = string.Empty;

        public ConfigurationMemberAttribute(string path, string defaultValue = null)
        {
            Path = path;
        }
    }
}