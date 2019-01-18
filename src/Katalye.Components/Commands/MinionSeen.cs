using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Notifications;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Katalye.Components.Commands
{
    public static class MinionSeen
    {
        public class Command : IRequest<Result>
        {
            public string Slug { get; set; }

            public DateTimeOffset Timestamp { get; set; }
        }

        public class Result
        {
            public Guid MinionId { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly KatalyeContext _context;
            private readonly IMediator _mediator;

            public Handler(KatalyeContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Trace($"Minion {message.Slug} seen at {message.Timestamp}.");

                using (var unit = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var minion = await _context.Minions
                                               .SingleOrDefaultAsync(x => x.MinionSlug == message.Slug, cancellationToken);

                    var minionDiscovered = minion == null;
                    if (minionDiscovered)
                    {
                        Logger.Info($"Minion {message.Slug} is unknown, it will be recorded.");
                        minion = new Minion
                        {
                            MinionSlug = message.Slug
                        };
                        _context.Minions.Add(minion);
                    }

                    minion.LastSeen = message.Timestamp;

                    await _context.SaveChangesAsync(cancellationToken);
                    unit.Commit();

                    if (minionDiscovered)
                    {
                        await _mediator.Publish(new MinionDiscovered.Notification
                        {
                            MinionSlug = message.Slug
                        }, cancellationToken);
                    }

                    return new Result
                    {
                        MinionId = minion.Id
                    };
                }
            }
        }
    }
}