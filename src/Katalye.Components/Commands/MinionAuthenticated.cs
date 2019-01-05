using System;
using System.Security.Cryptography;
using System.Text;
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
            [JsonProperty("_stamp"), JsonConverter(typeof(UtcTimeConverter))]
            public DateTime Timestamp { get; set; }

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

                var publicKeyHash = GetPublicKeyFingerprint(message.PublicKey);
                Logger.Debug($"Found public key fingerprint to be {publicKeyHash}.");

                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var minion = await _context.Minions.FindAsync(minionSeenResult.MinionId);
                    minion.LastAuthentication = message.Timestamp;

                    var authEvent = new MinionAuthenticationEvent
                    {
                        MinionId = minion.Id,
                        Action = message.Action,
                        PublicKey = message.PublicKey,
                        PublicKeyHash = publicKeyHash,
                        Success = message.Success,
                        Timestamp = message.Timestamp
                    };
                    _context.MinionAuthenticationEvents.Add(authEvent);

                    await _context.SaveChangesAsync();
                    unit.Commit();
                }

                Logger.Info("Processing authentication event for minion completed.");
            }

            private static string GetPublicKeyFingerprint(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return string.Empty;
                }

                var normalizedText = text.Replace("\r\n", "\n");

                var start = "-----BEGIN PUBLIC KEY-----\n".Length;
                var end = "-----END PUBLIC KEY-----".Length;

                var body = normalizedText.Substring(start, normalizedText.Length - start - end);

                using (var sha = new SHA256Managed())
                {
                    var textData = Encoding.UTF8.GetBytes(body);
                    var hash = sha.ComputeHash(textData);
                    var normalizedHash = BitConverter.ToString(hash).Replace('-', ':');
                    return normalizedHash;
                }
            }
        }
    }
}