using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Newtonsoft.Json;
using NLog;

namespace Katalye.Components.Commands
{
    public static class MinionAuthenticated
    {
        public class Command : IRequest<Result>
        {
            [JsonProperty("_stamp")]
            public DateTimeOffset Timestamp { get; set; }

            [JsonProperty("act")]
            public string Action { get; set; }

            [JsonProperty("id")]
            public string Slug { get; set; }

            [JsonProperty("pub")]
            public string PublicKey { get; set; }

            [JsonProperty("result")]
            public bool Success { get; set; }
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
            private readonly IMediator _mediator;

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient, IMediator mediator)
            {
                _context = context;
                _jobClient = jobClient;
                _mediator = mediator;
            }

            public Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                var jobId = _jobClient.Enqueue<Handler>(x => x.ProcessEvent(message));

                Logger.Debug($"Queued handling of new authentication event as job {jobId}.");

                return Task.FromResult(new Result());
            }

            [UsedImplicitly]
            public async Task ProcessEvent(Command message)
            {
                Logger.Info($"Processing authentication event for minion {message.Slug}.");

                var minionSeenResult = await _mediator.Send(new MinionSeen.Command
                {
                    Slug = message.Slug,
                    Timestamp = message.Timestamp
                });

                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var minion = await _context.Minions.FindAsync(minionSeenResult.MinionId);
                    minion.LastAuthentication = message.Timestamp;

                    var authEvent = new MinionAuthenticationEvent
                    {
                        MinionId = minion.Id,
                        Action = message.Action,
                        PublicKey = message.PublicKey,
                        Success = message.Success,
                        Timestamp = message.Timestamp
                    };
                    _context.MinionAuthenticationEvents.Add(authEvent);

                    await _context.SaveChangesAsync();
                    unit.Commit();
                }

                Logger.Info("Processing authentication event for minion completed.");
            }
        }
    }
}