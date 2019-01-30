using Castle.DynamicProxy;
using Katalye.Components;
using Katalye.Components.Configuration;
using Katalye.Components.Configuration.Providers;
using Katalye.Components.Processing;
using Lamar;

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