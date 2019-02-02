using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace Katalye.Components.Configuration.Providers
{
    [UsedImplicitly]
    public class DefaultsConfigurationProvider : IConfigurationProvider
    {
        private static readonly Lazy<Dictionary<string, string>> DefaultValues
            = new Lazy<Dictionary<string, string>>(CreateDefaultValues, LazyThreadSafetyMode.PublicationOnly);

        public int Priority => 1000;

        public string Name => "Defaults";

        public (bool Success, string Value) TryGet(string path)
        {
            var defaultValue = DefaultValues.Value[path];
            return (true, defaultValue);
        }

        private static Dictionary<string, string> CreateDefaultValues()
        {
            var defaultsType = typeof(IKatalyeConfiguration);
            var properties = defaultsType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var defaults = properties.Select(x => x.GetCustomAttribute<ConfigurationMemberAttribute>())
                                     .Where(x => x != null)
                                     .ToDictionary(x => x.Path, x => x.DefaultValue);
            return defaults;
        }
    }
}