using System.Collections.Generic;
using System.Linq;

namespace Katalye.Components.Configuration.Providers
{
    public class DbConfigurationProvider : IConfigurationProvider
    {
        private IReadOnlyDictionary<string, string> _lookup;

        public int Priority => 10;

        public (bool Success, string Value) TryGet(string path, string defaultValue)
        {
            var value = defaultValue;
            var ready = _lookup != null && _lookup.TryGetValue(path.ToLower(), out value);
            return (ready, value);
        }

        public void Load(IReadOnlyDictionary<string, string> dictionary)
        {
            _lookup = dictionary.ToDictionary(x => x.Key.ToLower(), x => x.Value);
        }
    }
}