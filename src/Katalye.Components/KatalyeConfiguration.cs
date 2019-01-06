using Microsoft.Extensions.Configuration;

namespace Katalye.Components
{
    public interface IKatalyeConfiguration
    {
        string SaltApiServer { get; }
        string SaltApiServiceUsername { get; }
        string SaltApiServicePassword { get; }
    }

    public class KatalyeConfiguration : IKatalyeConfiguration
    {
        private readonly IConfiguration _configuration;

        public string SaltApiServer => _configuration["Katalye:Salt:Api"];
        public string SaltApiServiceUsername => _configuration["Katalye:Salt:User"];
        public string SaltApiServicePassword => _configuration["Katalye:Salt:Password"];

        public KatalyeConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}