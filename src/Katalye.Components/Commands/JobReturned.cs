using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Components.Commands
{
    public static class JobReturned
    {
        public class Command : IRequest<Result>
        {
            public string Jid { get; set; }

            public string MinionId { get; set; }

            public Data Data { get; set; }
        }

        public class Data
        {
            [JsonProperty("fun_args")]
            public JArray FunArgs { get; set; }

            [JsonProperty("jid")]
            public string Jid { get; set; }

            [JsonProperty("return")]
            public JObject Return { get; set; }

            [JsonProperty("retcode")]
            public long ReturnCode { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("cmd")]
            public string Cmd { get; set; }

            [JsonProperty("_stamp")]
            public DateTimeOffset Stamp { get; set; }

            [JsonProperty("fun")]
            public string Fun { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly KatalyeContext _context;

            public Handler(KatalyeContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Return from {message.MinionId} for job with jid {message.Jid} occurred.");

                using (var unit = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var jobMinion = await _context.JobMinions
                                                  .Where(x => x.MinionId == message.MinionId && x.Job.Jid == message.Jid)
                                                  .SingleOrDefaultAsync(cancellationToken);

                    if (jobMinion == null)
                    {
                        Logger.Warn("Failed to reconcile job, ignoring event.");
                        return new Result();
                    }

                    Logger.Trace($"Reconciling return event with known minion to job relationship - minion id {jobMinion.Id} -> job id {jobMinion.JobId}.");

                    var returnEvent = new JobMinionReturnEvent
                    {
                        JobMinionId = jobMinion.Id,
                        ReturnCode = message.Data.ReturnCode,
                        Success = message.Data.Success,
                        Timestamp = message.Data.Stamp,
                        ReturnData = message.Data.Return
                    };
                    _context.JobMinionEvents.Add(returnEvent);

                    await _context.SaveChangesAsync(cancellationToken);

                    unit.Commit();
                }

                return new Result();
            }
        }
    }
}