using System;
using System.Collections.Generic;
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
    public static class GetJobByJid
    {
        public class Query : IRequest<Result>
        {
            public string Jid { get; set; }
        }

        public class Result
        {
            public string Jid { get; set; }
            public string Function { get; set; }
            public JArray Arguments { get; set; }
            public DateTimeOffset SeenAt { get; set; }
            public int CompletedCount { get; set; }
            public int SucceededCount { get; set; }

            public bool CreationEventExists { get; set; }
            public string User { get; set; }
            public IList<string> Targets { get; set; }
            public IList<string> Minions { get; set; }
            public IList<string> MissingMinions { get; set; }
            public string TargetType { get; set; }
            public DateTimeOffset? CreatedOn { get; set; }
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
                var result = await (from job in _context.Jobs.Where(x => x.Jid == message.Jid)
                                    from createEvent in _context.JobCreationEvents.Where(x => x.JobId == job.Id).DefaultIfEmpty()
                                    let minionCount = _context.MinionReturnEvents.Count(x => x.JobId == job.Id)
                                    let success = _context.MinionReturnEvents.Count(x => x.JobId == job.Id && x.Success != false && x.ReturnCode == 0)
                                    let hasCreationEvent = createEvent != null
                                    orderby job.CreatedOn descending
                                    select new Result
                                    {
                                        Jid = job.Jid,
                                        Function = job.Function,
                                        Arguments = job.Arguments,
                                        CreationEventExists = hasCreationEvent,
                                        SeenAt = job.CreatedOn,
                                        CompletedCount = minionCount,
                                        SucceededCount = success,
                                        CreatedOn = createEvent.Timestamp,
                                        TargetType = createEvent.TargetType,
                                        Targets = createEvent.Targets,
                                        User = createEvent.User,
                                        Minions = createEvent.Minions,
                                        MissingMinions = createEvent.MissingMinions
                                    }).SingleOrDefaultAsync(cancellationToken);

                return result;
            }
        }
    }
}