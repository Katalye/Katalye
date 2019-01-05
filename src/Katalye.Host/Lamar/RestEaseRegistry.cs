using Katalye.Components.Common;
using Lamar;
using Microsoft.Extensions.Configuration;
using RestEase;

namespace Katalye.Host.Lamar
{
    public class RestEaseRegistry : ServiceRegistry
    {
        public RestEaseRegistry()
        {
            For<ISaltApiClient>().Use(x =>
                                 {
                                     var configuration = x.GetInstance<IConfiguration>();
                                     var saltMasterServer = configuration["Katalye:Salt:Api"];
                                     return RestClient.For<ISaltApiClient>(saltMasterServer);
                                 })
                                 .Singleton();
        }
    }
}