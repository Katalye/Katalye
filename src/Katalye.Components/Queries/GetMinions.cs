using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;

namespace Katalye.Components.Queries
{
    public static class GetMinions
    {
        public class Query : IRequest<Result>, IPaginatedQuery
        {
            public int? Page { get; set; }
            public int? Size { get; set; }
        }

        public class Result : PagedResult<Minion>
        {
        }

        public class Minion
        {
            public string Id { get; set; }

            public DateTimeOffset? LastAuthenticated { get; set; }

            public DateTimeOffset? LastSeen { get; set; }
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
                var result = await _context.Minions
                                           .OrderBy(x => x.MinionSlug)
                                           .Select(x => new Minion
                                           {
                                               Id = x.MinionSlug,
                                               LastAuthenticated = x.LastAuthentication,
                                               LastSeen = x.LastSeen
                                           })
                                           .PageAsync(message, new Result());

                return result;
            }
        }
    }
}