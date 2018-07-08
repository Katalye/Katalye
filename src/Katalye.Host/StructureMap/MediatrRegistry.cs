using MediatR;

using StructureMap;

namespace Katalye.Host.StructureMap
{
    public class MediatrRegistry : Registry
    {
        public MediatrRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromPath("./", assembly => assembly.FullName.StartsWith("Katalye."));
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });
        }
    }
}