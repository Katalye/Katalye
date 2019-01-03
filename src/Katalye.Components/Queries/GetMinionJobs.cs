using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;

namespace Katalye.Components.Queries
{
    public static class GetMinionJobs
    {
        public class Query : IRequest<Result>, IPaginatedQuery
        {
            [Required]
            public string Id { get; set; }

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

            public bool Success { get; set; }

            public DateTimeOffset ReturnedOn { get; set; }
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
                return await _context.MinionReturnEvents
                                     .Where(x => x.Minion.MinionSlug == message.Id)
                                     .OrderByDescending(x => x.Timestamp)
                                     .Select(x => new Model
                                     {
                                         Jid = x.Job.Jid,
                                         Function = x.Job.Function,
                                         Success = x.Success,
                                         ReturnedOn = x.Timestamp,
                                     })
                                     .PageAsync(message, new Result());
            }
        }
    }
}