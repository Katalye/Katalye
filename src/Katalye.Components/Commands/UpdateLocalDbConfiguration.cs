using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Configuration.Providers;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            private readonly KatalyeContext _context;

            public Handler(DbConfigurationProvider configurationProvider, KatalyeContext context)
            {
                _configurationProvider = configurationProvider;
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info("Updating local db configuration cache.");
                var values = await _context.ServerSettings
                                           .Select(x => new {x.Key, x.Value})
                                           .ToListAsync(cancellationToken);
                var lookup = values.ToDictionary(x => x.Key, x => x.Value);

                _configurationProvider.Load(lookup);
                Logger.Info($"Local db configuration cache was updated sucessfully, a total of {lookup.Count} entries loaded.");

                return new Result();
            }
        }
    }
}