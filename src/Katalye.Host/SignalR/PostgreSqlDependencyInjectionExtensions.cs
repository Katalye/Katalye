using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Katalye.Host.SignalR
{
    public static class PostgreSqlDependencyInjectionExtensions
    {
        public static ISignalRServerBuilder AddPostgreSql(this ISignalRServerBuilder signalrBuilder)
        {
            signalrBuilder.Services.Configure<PostgreSqlOptions>(options => { });
            signalrBuilder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(PostgreSqlHubLifetimeManager<>));
            return signalrBuilder;
        }
    }
}