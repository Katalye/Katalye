using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Common;
using Katalye.Components.Configuration;
using MediatR;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Components.Commands.Jobs
{
    public static class FindJob
    {
        public class Command : IRequest<Result>
        {
            public string Jid { get; set; }

            [Required]
            public ICollection<string> Minions { get; set; }
        }

        public class Result
        {
            public ICollection<string> CompletedMinions { get; set; }

            public ICollection<string> PendingMinions { get; set; }

            public bool JobCompleted { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly ISaltApiClient _apiClient;
            private readonly IKatalyeConfiguration _configuration;

            public Handler(ISaltApiClient apiClient, IKatalyeConfiguration configuration)
            {
                _apiClient = apiClient;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Requesting job completion information for jid {message.Jid} regarding {message.Minions.Count} minions.");
                var minions = string.Join(",", message.Minions);

                var result = await _apiClient.FindJob(new CreateJobRequest
                {
                    Function = SaltCommands.SaltUtilFindJob,
                    TargetType = JobTarget.List,
                    Target = minions,
                    Client = JobClient.Local,
                    ExternalAuth = JobExternalAuth.Pam,
                    Password = _configuration.SaltApiServicePassword,
                    Username = _configuration.SaltApiServiceUsername,
                    Arguments = new List<string>
                    {
                        message.Jid
                    }
                });

                var minionResults = result.Return.First();

                var statuses = (from minion in message.Minions
                                from minionResult in minionResults.Where(x => x.Key == minion).Select(x => x.Value).DefaultIfEmpty()
                                let missing = minionResult == null
                                let pending = minionResult?.HasValues == true
                                let timeout = minionResult?.Type == JTokenType.Boolean && !minionResult.Value<bool>()
                                let status = CalculateJobStatus(missing, pending, timeout)
                                select new
                                {
                                    Minion = minion,
                                    Status = status
                                }).ToList();

                foreach (var minion in statuses.Where(x => x.Status == JobStatus.Timeout))
                {
                    Logger.Warn($"Minion {minion} timed out while finding job {message.Jid}.");
                }

                foreach (var minion in statuses.Where(x => x.Status == JobStatus.Missing))
                {
                    Logger.Error($"Failed to get job {message.Jid} status for minion {minion}. Report this error to a Katalye developer.");
                }

                var completedMinions = statuses.Where(x => x.Status == JobStatus.Completed).Select(x => x.Minion).ToList();
                var pendingMinions = statuses.Where(x => x.Status == JobStatus.Pending).Select(x => x.Minion).ToList();
                var jobCompleted = statuses.All(x => x.Status == JobStatus.Completed);

                Logger.Info($"Found {completedMinions.Count} completed, {pendingMinions.Count} pending with a completion status of {jobCompleted}.");

                return new Result
                {
                    CompletedMinions = completedMinions,
                    PendingMinions = pendingMinions,
                    JobCompleted = jobCompleted
                };
            }

            private JobStatus CalculateJobStatus(bool missing, bool pending, bool timeout)
            {
                if (missing)
                {
                    return JobStatus.Missing;
                }

                if (timeout)
                {
                    return JobStatus.Timeout;
                }

                if (pending)
                {
                    return JobStatus.Pending;
                }

                return JobStatus.Completed;
            }
        }

        private enum JobStatus
        {
            [UsedImplicitly]
            Unknown,
            Pending,
            Completed,
            Missing,
            Timeout
        }
    }
}