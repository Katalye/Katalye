using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components.Queries
{
    public static class GetSearchGrains
    {
        public class Query : IRequest<Result>
        {
            /// <summary>
            /// HACK - Should be in the format of a,b
            /// </summary>
            [CanBeNull]
            public IList<string> Search { get; set; }
        }

        public class Result
        {
            public ICollection<string> GrainPaths { get; set; }
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
                var valuePairs = (message.Search ?? new List<string>())
                                 .Select(x => x.Split(new[] {','}, 2))
                                 .Select(x => new
                                 {
                                     Key = x.First(),
                                     Value = x.Last()
                                 });

                var query = from grain in _context.MinionGrains
                            join value in _context.MinionGrainValues on grain.Id equals value.MinionGrainId
                            select new
                            {
                                grain.Path,
                                value.Value,
                                grain.Generation
                            };

                foreach (var pair in valuePairs)
                {
                    query = query.Where(x => x.Path == pair.Key && x.Value.StartsWith(pair.Value));
                }

                // BUG https://github.com/aspnet/EntityFrameworkCore/issues/14332
                // Merge these statements when fixed.

                var generations = await query.Select(x => x.Generation)
                                             .Distinct()
                                             .ToListAsync(cancellationToken);

                var paths = await _context.MinionGrains.Where(x => generations.Contains(x.Generation))
                                          .Select(x => x.Path)
                                          .Where(x => !x.Contains(".")) // HACK Need to detect dynamic grain paths.
                                          .Where(x => !x.Contains("["))
                                          .Distinct()
                                          .ToListAsync(cancellationToken);

                return new Result
                {
                    GrainPaths = paths
                };
            }
        }
    }
}