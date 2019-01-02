using Lamar;
using MediatR;

namespace Katalye.Host.StructureMap
{
    public class MediatrRegistry : ServiceRegistry
    {
        public MediatrRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssemblyContainingType<IMediator>();
                scanner.WithDefaultConventions();
            });

            For<ServiceFactory>().Use(x => x.GetInstance);

            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromPath("./", assembly => assembly.FullName.StartsWith("Katalye."));
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });
        }
    }
}