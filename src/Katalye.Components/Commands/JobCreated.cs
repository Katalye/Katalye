using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
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
    public static class JobCreated
    {
        public class Command : IRequest<Result>
        {
            public string Jid { get; set; }

            public CreationData Data { get; set; }
        }

        public class CreationData
        {
            [JsonProperty("tgt_type")]
            public string TargetType { get; set; }

            [JsonProperty("jid")]
            public string Jid { get; set; }

            [JsonProperty("tgt")]
            [JsonConverter(typeof(SingleOrArrayConverter<string>))]
            public List<string> Target { get; set; }

            [JsonProperty("_stamp")]
            public DateTimeOffset TimeStamp { get; set; }

            [JsonProperty("user")]
            public string User { get; set; }

            [JsonProperty("arg")]
            public JArray Arguments { get; set; }

            [JsonProperty("fun")]
            public string Function { get; set; }

            [JsonProperty("minions")]
            public List<string> Minions { get; set; }

            [JsonProperty("missing")]
            public List<string> Missing { get; set; }
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
                Logger.Info($"A new job {message.Jid} was created.");

                _jobClient.Enqueue<Handler>(x => x.ProcessEvent(message));

                return Task.FromResult(new Result());
            }

            [UsedImplicitly]
            public async Task ProcessEvent(Command message)
            {
                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var job = await _context.Jobs.SingleOrDefaultAsync(x => x.Jid == message.Jid);

                    var jobExists = job != null;
                    if (!jobExists)
                    {
                        job = new Job
                        {
                            Jid = message.Jid,
                            Function = message.Data.Function,
                            Arguments = message.Data.Arguments
                        };
                        _context.Jobs.Add(job);
                        await _context.SaveChangesAsync();
                    }

                    var creationEvent = new JobCreationEvent
                    {
                        JobId = job.Id,
                        Minions = message.Data.Minions,
                        MissingMinions = message.Data.Missing,
                        Targets = message.Data.Target,
                        TargetType = message.Data.TargetType,
                        User = message.Data.User,
                        TimeStamp = message.Data.TimeStamp
                    };
                    _context.JobCreationEvents.Add(creationEvent);
                    await _context.SaveChangesAsync();

                    unit.Commit();
                }

                Logger.Info($"Job {message.Jid} was committed.");
            }
        }
    }
}