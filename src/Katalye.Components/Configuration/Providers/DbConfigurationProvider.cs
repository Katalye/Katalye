using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Katalye.Components.Configuration.Providers
{
    [UsedImplicitly]
    public class DbConfigurationProvider : IConfigurationProvider
    {
        private IReadOnlyDictionary<string, string> _lookup;

        public int Priority => 100;

        public string Name => "Database";

        public (bool Success, string Value) TryGet(string path)
        {
            string value = null;
            var ready = _lookup != null
                        && _lookup.TryGetValue(path.ToLower(), out value)
                        && !string.IsNullOrWhiteSpace(value);
            return (ready, value);
        }

        public void Load(IReadOnlyDictionary<string, string> dictionary)
        {
            _lookup = dictionary.ToDictionary(x => x.Key.ToLower(), x => x.Value);
        }
    }
}