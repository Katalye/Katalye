using System;

namespace Katalye.Components.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationMemberAttribute : Attribute
    {
        public string Path { get; set; }

        public string DefaultValue { get; set; }

        public ConfigurationMemberAttribute(string path, string defaultValue = "")
        {
            Path = path;
            DefaultValue = defaultValue;
        }
    }
}