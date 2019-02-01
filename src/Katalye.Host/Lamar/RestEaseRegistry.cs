using System;
using System.Net.Http;
using System.Threading.Tasks;
using Katalye.Components.Common;
using Katalye.Components.Configuration;
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
                                     return RestClient.For<ISaltApiClient>(saltMasterServer,
                                         (request, token) => RequestModifier(request, configuration));
                                 })
                                 .Singleton();
        }

        private static Task RequestModifier(HttpRequestMessage request, IKatalyeConfiguration configuration)
        {
            var scheme = configuration.SaltApiServer.Scheme;
            var host = configuration.SaltApiServer.Host;
            var port = configuration.SaltApiServer.Port;

            var builder = new UriBuilder(request.RequestUri)
            {
                Scheme = scheme,
                Host = host,
                Port = port
            };

            request.RequestUri = builder.Uri;

            return Task.CompletedTask;
        }
    }
}