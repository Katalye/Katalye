using Castle.DynamicProxy;
using Katalye.Components;
using Katalye.Components.Configuration;
using Katalye.Components.Configuration.Providers;
using Katalye.Components.Processing;
using Katalye.Data;
using Lamar;
using Microsoft.Extensions.Configuration;
using Npgsql;
using IConfigurationProvider = Katalye.Components.Configuration.Providers.IConfigurationProvider;

namespace Katalye.Host.Lamar
{
    public class KatalyeRegistry : ServiceRegistry
    {
        public KatalyeRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromPath("./", assembly => assembly.FullName.StartsWith("Katalye."));
                scanner.AddAllTypesOf<ProcessingServer>();
                scanner.WithDefaultConventions();
            });

            For<NpgsqlConnection>().Use(x =>
            {
                var configuration = x.GetInstance<IConfiguration>();
                var connectionString = configuration.GetConnectionString(nameof(KatalyeContext));
                return new NpgsqlConnection(connectionString);
            });

            For<IProxyGenerator>().Use<ProxyGenerator>();
            For<IKatalyeConfiguration>().Use(x =>
            {
                var router = x.GetInstance<ConfigurationRouter>();
                return router.CreateConfiguration();
            });
            For<IConfigurationProvider>().Use<BuiltInProvider>();
            For<IConfigurationProvider>().Use<DbConfigurationProvider>();
        }
    }
}