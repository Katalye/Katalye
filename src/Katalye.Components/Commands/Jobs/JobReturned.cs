using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Components.Commands.Minions;
using Katalye.Components.Common;
using Katalye.Components.Notifications;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Components.Commands.Jobs
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
            public long? ReturnCode { get; set; }

            [JsonProperty("success")]
            public bool? Success { get; set; }

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

                var stats = ParseStats(message);

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
                        ReturnData = message.Data.Return,
                        ChangedCount = stats.ChangedCount,
                        FailedCount = stats.FailedCount,
                        SuccessCount = stats.SuccessCount
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
                await _mediator.PublishEvent(
                    $"v1:minions:{message.MinionSlug}:jobs:{job.JobId}:returned",
                    ("minionId", message.MinionSlug),
                    ("jid", message.Jid)
                );
            }

            private (int ChangedCount, int FailedCount, int SuccessCount) ParseStats(Command message)
            {
                int changed;
                int failed;
                int success;

                if (new[] {"state.highstate", "state.apply"}.Contains(message.Data.Function))
                {
                    Logger.Debug("Detected job as state data, will process return data.");

                    try
                    {
                        var returnData = message.Data.Return.ToObject<Dictionary<string, MinimumStateProgress>>();

                        Logger.Debug($"Found {returnData.Count} progress reports.");

                        changed = returnData.Count(x => x.Value.Changes != null && x.Value.Changes.ToString() != "{}");
                        failed = returnData.Count(x => !x.Value.RunResult);
                        success = returnData.Count(x => x.Value.RunResult);
                    }
                    catch (JsonSerializationException e)
                    {
                        Logger.Warn(e, "Job return data failed to deserialize.");
                        changed = 0;
                        failed = 0;
                        success = 0;
                    }
                }
                else
                {
                    // Job status is often incorrect.
                    // https://github.com/saltstack/salt/issues/45775
                    Logger.Debug("Could not detect function type for parsing, using defaults.");
                    if (message.Data.ReturnCode == 0 || message.Data.Success == true)
                    {
                        success = 1;
                        failed = 0;
                    }
                    else
                    {
                        failed = 1;
                        success = 0;
                    }

                    changed = -1;
                }

                return (changed, failed, success);
            }

            private class MinimumStateProgress
            {
                [JsonProperty("comment")]
                public string Comment { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("start_time")]
                public DateTimeOffset StartTime { get; set; }

                [JsonProperty("result")]
                public bool RunResult { get; set; }

                [JsonProperty("duration")]
                public double Duration { get; set; }

                [JsonProperty("__run_num__")]
                public long RunNumber { get; set; }

                [JsonProperty("__sls__")]
                public string Sls { get; set; }

                [JsonProperty("changes")]
                public JToken Changes { get; set; }

                [JsonProperty("__id__")]
                public string Id { get; set; }
            }
        }
    }
}