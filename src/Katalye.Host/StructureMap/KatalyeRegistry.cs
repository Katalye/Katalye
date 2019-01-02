using Lamar;

namespace Katalye.Host.StructureMap
{
    public class KatalyeRegistry : ServiceRegistry
    {
        public KatalyeRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromPath("./", assembly => assembly.FullName.StartsWith("Katalye."));
                scanner.WithDefaultConventions();
            });
        }
    }
}