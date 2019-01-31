using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katalye.Data;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components.Configuration.Providers
{
    public class DbConfigurationProvider : IConfigurationProvider
    {
        private readonly KatalyeContext _context;
        private IDictionary<string, string> _lookup;

        public int Priority => 10;

        public DbConfigurationProvider(KatalyeContext context)
        {
            _context = context;
        }

        public (bool Success, string Value) TryGet(string path, string defaultValue)
        {
            var value = defaultValue;
            var ready = _lookup != null && _lookup.TryGetValue(path, out value);
            return (ready, value);
        }

        public async Task<int> Load()
        {
            var values = await _context.ServerConfigurationValues
                                       .Select(x =>
                                           new
                                           {
                                               x.Key,
                                               x.Value
                                           })
                                       .ToListAsync();

            _lookup = values.ToDictionary(x => x.Key, x => x.Value);

            return _lookup.Count;
        }
    }
}