using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Common;
using MediatR;
using NLog;

namespace Katalye.Components.Commands.Jobs
{
    public static class CreateJob
    {
        public class Command : IRequest<Result>
        {
            public JobClient Client { get; set; }

            [Required]
            public string Target { get; set; }

            public IList<string> Arguments { get; set; }

            [Required]
            public string Function { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public class Result
        {
            public string Jid { get; set; }

            public ICollection<string> TargetedMinions { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly ISaltApiClient _apiClient;

            public Handler(ISaltApiClient apiClient)
            {
                _apiClient = apiClient;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info($"Creating job for function {message.Function} targting minion {message.Target}.");

                var result = await _apiClient.CreateJob(new CreateJobRequest
                {
                    Function = message.Function,
                    TargetType = JobTarget.List,
                    Target = message.Target,
                    Client = message.Client,
                    ExternalAuth = JobExternalAuth.Pam,
                    Password = message.Password,
                    Username = message.Username,
                    Arguments = message.Arguments
                });

                var success = result.Return.Any();
                if (!success)
                {
                    throw new Exception("Failed to create job.");
                }

                var returnData = result.Return.Single();

                Logger.Info($"Job {returnData.Jid} was created.");

                return new Result
                {
                    Jid = returnData.Jid,
                    TargetedMinions = returnData.Minions
                };
            }
        }
    }
}