using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Configuration.Providers;
using MediatR;
using NLog;

namespace Katalye.Components.Commands
{
    public static class UpdateLocalDbConfiguration
    {
        public class Command : IRequest<Result>
        {
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly DbConfigurationProvider _configurationProvider;

            public Handler(DbConfigurationProvider configurationProvider)
            {
                _configurationProvider = configurationProvider;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info("Updating local db configuration cache.");
                var count = await _configurationProvider.Load();
                Logger.Info($"Local db configuration cache was updated sucessfully, a total of {count} entries loaded.");

                return new Result();
            }
        }
    }
}