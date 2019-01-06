using Katalye.Components;
using Katalye.Components.Common;
using Lamar;
using RestEase;

namespace Katalye.Host.Lamar
{
    public class RestEaseRegistry : ServiceRegistry
    {
        public RestEaseRegistry()
        {
            For<ISaltApiClient>().Use(x =>
                                 {
                                     var configuration = x.GetInstance<IKatalyeConfiguration>();
                                     var saltMasterServer = configuration.SaltApiServer;
                                     return RestClient.For<ISaltApiClient>(saltMasterServer);
                                 })
                                 .Singleton();
        }
    }
}