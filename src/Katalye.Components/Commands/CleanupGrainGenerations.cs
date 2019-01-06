using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Katalye.Components.Commands
{
    public static class CleanupGrainGenerations
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

            private readonly KatalyeContext _context;
            private readonly IBackgroundJobClient _jobClient;

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient)
            {
                _context = context;
                _jobClient = jobClient;
            }

            public Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                _jobClient.Enqueue<Handler>(x => x.Process());

                return Task.FromResult(new Result());
            }

            [UsedImplicitly]
            public async Task Process()
            {
                Logger.Info("Processing cleanup of grain generations.");

                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var grainIds = await (from grain in _context.MinionGrains
                                          from minion in _context.Minions.Where(x => x.GrainGeneration == grain.Generation).DefaultIfEmpty()
                                          where minion == null
                                          select grain.Id).ToListAsync();

                    var grains = grainIds.Select(x => new MinionGrain
                    {
                        Id = x
                    });
                    _context.MinionGrains.RemoveRange(grains);
                    await _context.SaveChangesAsync();

                    unit.Commit();

                    Logger.Info($"Removed {grainIds.Count} grains from old generations.");
                }
            }
        }
    }
}