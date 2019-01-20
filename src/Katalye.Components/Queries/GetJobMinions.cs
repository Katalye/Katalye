using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;

namespace Katalye.Components.Queries
{
    public static class GetJobMinions
    {
        public class Query : IRequest<Result>, IPaginatedQuery
        {
            public string Jid { get; set; }
            public int? Page { get; set; }
            public int? Size { get; set; }
        }

        public class Result : PagedResult<Model>
        {
        }

        public class Model
        {
            public string MinionId { get; set; }

            public DateTimeOffset ReturnedAt { get; set; }

            public bool? Success { get; set; }

            public long? ReturnCode { get; set; }

            public int SuccessCount { get; set; }

            public int FailedCount { get; set; }

            public int ChangedCount { get; set; }
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
                                    from returnEvent in _context.MinionReturnEvents.Where(x => x.JobId == job.Id)
                                    let minion = returnEvent.Minion
                                    orderby returnEvent.Timestamp descending
                                    select new Model
                                    {
                                        MinionId = minion.MinionSlug,
                                        ReturnedAt = returnEvent.Timestamp,
                                        ReturnCode = returnEvent.ReturnCode,
                                        Success = returnEvent.Success,
                                        ChangedCount = returnEvent.ChangedCount,
                                        FailedCount = returnEvent.FailedCount,
                                        SuccessCount = returnEvent.SuccessCount
                                    }).PageAsync(message, new Result());

                return result;
            }
        }
    }
}