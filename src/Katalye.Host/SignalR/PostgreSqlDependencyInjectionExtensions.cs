using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Katalye.Host.SignalR
{
    public static class PostgreSqlDependencyInjectionExtensions
    {
        public static ISignalRServerBuilder AddPostgreSql(this ISignalRServerBuilder signalrBuilder, Action<PostgreSqlOptions> options)
        {
            signalrBuilder.Services.Configure(options);
            signalrBuilder.Services.AddSingleton(typeof(HubLifetimeManager<>), typeof(PostgreSqlHubLifetimeManager<>));
            return signalrBuilder;
        }
    }
}