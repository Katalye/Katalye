using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Katalye.Components.Queries
{
    public static class GetMinionJobByJid
    {
        public class Query : IRequest<Result>
        {
            [Required]
            public string Id { get; set; }

            [Required]
            public string Jid { get; set; }
        }

        public class Result
        {
            public string Jid { get; set; }

            public string Function { get; set; }

            public JArray Arguments { get; set; }

            public JToken ReturnData { get; set; }

            public bool? Success { get; set; }

            public DateTimeOffset ReturnedOn { get; set; }

            public string TargetType { get; set; }

            public string User { get; set; }

            public List<string> Targets { get; set; }

            public bool CreationEventExists { get; set; }

            public DateTimeOffset? CreatedOn { get; set; }

            public List<string> Minions { get; set; }

            public List<string> MissingMinions { get; set; }

            public int SuccessCount { get; set; }

            public int FailedCount { get; set; }

            public int ChangedCount { get; set; }

            public long? ReturnCode { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly KatalyeContext _context;

            public Handler(KatalyeContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = await (from returnEvent in _context.MinionReturnEvents
                                    from creationEvent in _context.JobCreationEvents.Where(x => x.JobId == returnEvent.JobId).DefaultIfEmpty()
                                    let job = returnEvent.Job
                                    where job.Jid == message.Jid && returnEvent.Minion.MinionSlug == message.Id
                                    select new Result
                                    {
                                        Jid = job.Jid,
                                        Function = job.Function,
                                        Arguments = job.Arguments,
                                        Success = returnEvent.Success,
                                        ReturnCode = returnEvent.ReturnCode,
                                        ReturnedOn = returnEvent.Timestamp,
                                        TargetType = creationEvent.TargetType,
                                        User = creationEvent.User,
                                        Targets = creationEvent.Targets,
                                        CreationEventExists = creationEvent != null,
                                        CreatedOn = creationEvent.Timestamp,
                                        Minions = creationEvent.Minions,
                                        MissingMinions = creationEvent.MissingMinions,
                                        ReturnData = returnEvent.ReturnData,
                                        SuccessCount = returnEvent.SuccessCount,
                                        ChangedCount = returnEvent.ChangedCount,
                                        FailedCount = returnEvent.FailedCount
                                    }).SingleOrDefaultAsync(cancellationToken);
                return result;
            }
        }
    }
}