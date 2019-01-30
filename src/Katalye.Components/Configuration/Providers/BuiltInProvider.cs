using Microsoft.Extensions.Configuration;

namespace Katalye.Components.Configuration.Providers
{
    public class BuiltInProvider : IConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        public int Priority => 100;

        public BuiltInProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool Success, string Value) TryGet(string path, string defaultValue)
        {
            var value = _configuration.GetValue(path, defaultValue);
            return (true, value);
        }
    }
}