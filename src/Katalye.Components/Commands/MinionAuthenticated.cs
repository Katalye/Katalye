using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient)
            {
                _context = context;
                _jobClient = jobClient;
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

                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var minion = await _context.Minions
                                               .SingleOrDefaultAsync(x => x.MinionSlug == message.Slug);

                    var minionExists = minion != null;
                    if (!minionExists)
                    {
                        Logger.Info($"Minion {message.Slug} is unknown, it will be recorded.");
                        minion = new Minion
                        {
                            LastAuthentication = message.Timestamp,
                            MinionSlug = message.Slug
                        };
                        _context.Minions.Add(minion);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        minion.LastAuthentication = message.Timestamp;
                    }

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