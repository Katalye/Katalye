using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
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

            [JsonProperty("tgt"), JsonConverter(typeof(SingleOrArrayConverter<string>))]
            public List<string> Target { get; set; }

            [JsonProperty("_stamp"), JsonConverter(typeof(UtcTimeConverter))]
            public DateTime Timestamp { get; set; }

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
            private readonly IMediator _mediator;

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient, IMediator mediator)
            {
                _context = context;
                _jobClient = jobClient;
                _mediator = mediator;
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
                Logger.Trace($"Processing job created event for jid {message.Jid}.");

                var job = await _mediator.Send(new JobSeen.Command
                {
                    Jid = message.Jid,
                    Function = message.Data.Function,
                    Arguments = message.Data.Arguments
                });

                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var creationEvent = new JobCreationEvent
                    {
                        JobId = job.JobId,
                        Minions = message.Data.Minions,
                        MissingMinions = message.Data.Missing,
                        Targets = message.Data.Target,
                        TargetType = message.Data.TargetType,
                        User = message.Data.User,
                        Timestamp = message.Data.Timestamp
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