using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Katalye.Data;
using Katalye.Data.Entities;

using MediatR;

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

            public Handler(KatalyeContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"A new job {message.Jid} was created.");

                using (var unit = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var job = new Job
                    {
                        Jid = message.Jid,
                        Function = message.Data.Function,
                        TargetType = message.Data.TargetType,
                        Target = message.Data.Target,
                        TimeStamp = message.Data.TimeStamp,
                        User = message.Data.User,
                        Arguments = message.Data.Arguments,
                        MissingMinions = message.Data.Missing
                    };
                    _context.Jobs.Add(job);
                    await _context.SaveChangesAsync(cancellationToken);

                    var jobMinions = message.Data.Minions.Select(x => new JobMinion
                    {
                        MinionId = x,
                        JobId = job.Id
                    });
                    _context.JobMinions.AddRange(jobMinions);
                    await _context.SaveChangesAsync(cancellationToken);

                    unit.Commit();
                }

                Logger.Info($"Job {message.Jid} was committed.");

                return new Result();
            }
        }
    }
}