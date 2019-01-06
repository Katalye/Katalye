using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Common;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Katalye.Components.Commands
{
    public static class RefreshMinionGrains
    {
        public class Command : IRequest<Result>
        {
            public TimeSpan Age { get; set; }
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly KatalyeContext _context;
            private readonly IMediator _mediator;
            private readonly IKatalyeConfiguration _configuration;

            public Handler(KatalyeContext context, IMediator mediator, IKatalyeConfiguration configuration)
            {
                _context = context;
                _mediator = mediator;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Minion grain refresh in-progress, locating grain data that is older than {message.Age}.");

                var cutoff = DateTimeOffset.Now - message.Age;
                var minions = await _context.Minions
                                            .Where(x => x.LastGrainRefresh == null || x.LastGrainRefresh < cutoff)
                                            .Select(x => x.MinionSlug)
                                            .ToListAsync(cancellationToken);

                if (minions.Any())
                {
                    var targetList = string.Join(",", minions);

                    Logger.Info($"Requesting refreshes for {minions} minions.");

                    var result = await _mediator.Send(new CreateJob.Command
                    {
                        Client = JobClient.LocalAsync,
                        Function = "grains.items",
                        Target = targetList,
                        Username = _configuration.SaltApiServiceUsername,
                        Password = _configuration.SaltApiServicePassword
                    }, cancellationToken);

                    Logger.Info($"Salt job {result.Jid} created.");
                }
                else
                {
                    Logger.Info("No minions require a grain refresh.");
                }

                return new Result();
            }
        }
    }
}