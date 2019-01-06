using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Components.Notifications;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
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
            public JToken Return { get; set; }

            [JsonProperty("retcode")]
            public long ReturnCode { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("cmd")]
            public string Cmd { get; set; }

            [JsonProperty("_stamp"), JsonConverter(typeof(UtcTimeConverter))]
            public DateTime Timestamp { get; set; }

            [JsonProperty("fun")]
            public string Function { get; set; }

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
            private readonly IMediator _mediator;

            public Handler(KatalyeContext context, IBackgroundJobClient jobClient, IMediator mediator)
            {
                _context = context;
                _jobClient = jobClient;
                _mediator = mediator;
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
                Logger.Trace($"Processing job created event for jid {message.Jid}.");

                var minion = await _mediator.Send(new MinionSeen.Command
                {
                    Slug = message.MinionSlug,
                    Timestamp = message.Data.Timestamp
                });
                var job = await _mediator.Send(new JobSeen.Command
                {
                    Jid = message.Jid,
                    Function = message.Data.Function,
                    Arguments = message.Data.FunctionArguments
                });

                Guid returnEventId;
                using (var unit = await _context.Database.BeginTransactionAsync())
                {
                    var returnEvent = new MinionReturnEvent
                    {
                        MinionId = minion.MinionId,
                        JobId = job.JobId,
                        ReturnCode = message.Data.ReturnCode,
                        Success = message.Data.Success,
                        Timestamp = message.Data.Timestamp,
                        ReturnData = message.Data.Return
                    };
                    _context.MinionReturnEvents.Add(returnEvent);
                    await _context.SaveChangesAsync();

                    returnEventId = returnEvent.Id;

                    unit.Commit();
                }

                await _mediator.Publish(new JobReturnSaved.Notification
                {
                    Function = message.Data.Function,
                    MinionReturnEventId = returnEventId
                });
            }
        }
    }
}