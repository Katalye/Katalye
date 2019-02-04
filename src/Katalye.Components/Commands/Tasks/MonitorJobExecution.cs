using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using JetBrains.Annotations;
using Katalye.Components.Commands.Jobs;
using Katalye.Data;
using Katalye.Data.Common;
using Katalye.Data.Entities;
using MediatR;
using NLog;

namespace Katalye.Components.Commands.Tasks
{
    public static class MonitorJobExecution
    {
        public class Command : IRequest<Result>
        {
            [Required]
            public string Tag { get; set; }

            [Required]
            public string Jid { get; set; }

            [Required]
            public ICollection<string> Minions { get; set; }

            public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(15);
        }

        public class Result
        {
            public Guid TaskId { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly IMediator _mediator;
            private readonly IBackgroundJobClient _jobClient;
            private readonly KatalyeContext _context;

            public Handler(IMediator mediator, IBackgroundJobClient jobClient, KatalyeContext context)
            {
                _mediator = mediator;
                _jobClient = jobClient;
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                using (var unit = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var task = new AdHocTask
                    {
                        Tag = message.Tag,
                        Metadata = new Dictionary<string, string>
                        {
                            {"jid", message.Jid},
                            {"minions", string.Join(",", message.Minions)}
                        },
                        StartedOn = DateTimeOffset.Now,
                        Status = BackgroundTaskStatus.Queued
                    };

                    _context.AdHocTasks.Add(task);
                    await _context.SaveChangesAsync(cancellationToken);

                    var taskId = task.Id;
                    Logger.Info($"Queuing monitoring of job {message.Jid}'s execution, tracking with task {taskId}.");

                    _jobClient.Enqueue<Handler>(x => x.Process(message, taskId));

                    unit.Commit();

                    return new Result
                    {
                        TaskId = taskId
                    };
                }
            }

            [UsedImplicitly]
            public async Task Process(Command message, Guid taskId)
            {
                var task = await _context.AdHocTasks.FindAsync(taskId) ?? throw new Exception($"Task {taskId} does not exist.");

                task.Status = BackgroundTaskStatus.Processing;
                await _context.SaveChangesAsync();

                var timeoutTime = DateTimeOffset.Now + message.Timeout;
                Logger.Info($"Starting monitoring session for job {message.Jid}. Timeout will occur at {timeoutTime}.");

                var completed = false;
                var timeout = false;

                Logger.Info("Requesting job status information from minions.");
                do
                {
                    var result = await _mediator.Send(new FindJob.Command
                    {
                        Jid = message.Jid,
                        Minions = message.Minions
                    });

                    if (result.JobCompleted)
                    {
                        completed = true;
                    }
                    else if (result.AllMinionsTimedOut)
                    {
                        Logger.Warn("All minions timed out, assuming failure.");
                        timeout = true;
                    }
                    else
                    {
                        timeout = timeoutTime < DateTimeOffset.Now;

                        Logger.Debug($"Still waiting on completion of [{string.Join(",", result.PendingMinions)}] minions.");
                        if (!timeout)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));
                        }
                    }
                } while (!(completed || timeout));

                if (completed)
                {
                    Logger.Info("All minions signaled completion.");
                    task.Status = BackgroundTaskStatus.Succeeded;
                }
                else
                {
                    Logger.Warn("Timed out while waiting for job completion.");
                    task.Status = BackgroundTaskStatus.Failed;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}