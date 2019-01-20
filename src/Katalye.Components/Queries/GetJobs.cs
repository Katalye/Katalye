using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Newtonsoft.Json.Linq;

namespace Katalye.Components.Queries
{
    public static class GetJobs
    {
        public class Query : IRequest<Result>, IPaginatedQuery
        {
            public int? Page { get; set; }
            public int? Size { get; set; }
        }

        public class Result : PagedResult<Model>
        {
        }

        public class Model
        {
            public string Jid { get; set; }
            public string Function { get; set; }
            public JArray Arguments { get; set; }
            public bool HasCreationEvent { get; set; }
            public DateTimeOffset SeenAt { get; set; }
            public int CompletedCount { get; set; }
            public int SucceededCount { get; set; }
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
                var result = await (from job in _context.Jobs
                                    from createEvent in _context.JobCreationEvents.Where(x => x.JobId == job.Id).DefaultIfEmpty()
                                    let minionCount = _context.MinionReturnEvents.Count(x => x.JobId == job.Id)
                                    let success = _context.MinionReturnEvents.Count(x => x.JobId == job.Id && x.Success == true || x.ReturnCode == 0)
                                    let hasCreationEvent = createEvent != null
                                    select new Model
                                    {
                                        Jid = job.Jid,
                                        Function = job.Function,
                                        Arguments = job.Arguments,
                                        HasCreationEvent = hasCreationEvent,
                                        SeenAt = job.CreatedOn,
                                        CompletedCount = minionCount,
                                        SucceededCount = success
                                    }).PageAsync(message, new Result());

                return result;
            }
        }
    }
}