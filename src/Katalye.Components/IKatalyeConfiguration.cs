using Katalye.Components.Configuration;

namespace Katalye.Components
{
    public interface IKatalyeConfiguration
    {
        [ConfigurationMember("Katalye:Salt:Api")]
        string SaltApiServer { get; }

        [ConfigurationMember("Katalye:Salt:User")]
        string SaltApiServiceUsername { get; }

        [ConfigurationMember("Katalye:Salt:Password")]
        string SaltApiServicePassword { get; }
    }
}