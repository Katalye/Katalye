using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Components.Exceptions;
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

            public string MinionSlug { get; set; }

            public Data Data { get; set; }
        }

        public class Data
        {
            [JsonProperty("fun_args")]
            public JArray FunctionArguments { get; set; }

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
            public DateTimeOffset Timestamp { get; set; }

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
            private readonly IBackgroundJobClient _jobClient;

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient)
            {
                _context = context;
                _jobClient = jobClient;
            }

            public Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Return from {message.MinionSlug} for job with jid {message.Jid} occurred.");

                _jobClient.Enqueue<Handler>(x => x.ProcessEvent(message));

                return Task.FromResult(new Result());
            }

            [UsedImplicitly]
            public async Task ProcessEvent(Command message)
            {
                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var minion = await _context.Minions
                                               .Where(x => x.MinionSlug == message.MinionSlug)
                                               .SingleOrDefaultAsync();

                    if (minion == null)
                    {
                        Logger.Info($"Minion called {message.MinionSlug} does not exist, delaying event processing until minion re-authenicates.");
                        throw new MinionUnknownException(message.MinionSlug);
                    }

                    var job = await _context.Jobs.SingleOrDefaultAsync(x => x.Jid == message.Jid);
                    var jobExists = job != null;

                    if (!jobExists)
                    {
                        job = new Job
                        {
                            Arguments = message.Data.FunctionArguments,
                            Function = message.Data.Fun,
                            Jid = message.Jid
                        };
                        _context.Jobs.Add(job);
                        await _context.SaveChangesAsync();
                    }

                    var returnEvent = new MinionReturnEvent
                    {
                        MinionId = minion.Id,
                        JobId = job.Id,
                        ReturnCode = message.Data.ReturnCode,
                        Success = message.Data.Success,
                        Timestamp = message.Data.Timestamp,
                        ReturnData = message.Data.Return
                    };
                    _context.JobMinionEvents.Add(returnEvent);
                    await _context.SaveChangesAsync();

                    unit.Commit();
                }
            }
        }
    }
}