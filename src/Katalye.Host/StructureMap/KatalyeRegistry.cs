using StructureMap;

namespace Katalye.Host.StructureMap
{
    public class KatalyeRegistry : Registry
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