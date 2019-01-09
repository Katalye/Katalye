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

namespace Katalye.Components.Queries
{
    public static class GetMinionBySlug
    {
        public class Query : IRequest<Result>
        {
            [Required]
            public string Id { get; set; }
        }

        public class Result
        {
            public string Id { get; set; }

            public DateTimeOffset? LastAuthentication { get; set; }

            public DateTimeOffset? LastSeen { get; set; }

            public Dictionary<string, ICollection<string>> Grains { get; set; }
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
                                           .Where(x => x.MinionSlug == message.Id)
                                           .Select(x => new Result
                                           {
                                               Id = x.MinionSlug,
                                               LastAuthentication = x.LastAuthentication,
                                               LastSeen = x.LastSeen
                                           })
                                           .SingleOrDefaultAsync(cancellationToken);

                if (result != null)
                {
                    var grains = await _context.MinionGrains
                                               .Where(x => x.Minion.MinionSlug == message.Id)
                                               .Where(x => x.Minion.GrainGeneration == x.Generation)
                                               .Select(x => new
                                               {
                                                   x.Path,
                                                   x.Values
                                               })
                                               .ToListAsync(cancellationToken);

                    result.Grains = grains.ToDictionary(x => x.Path, x => (ICollection<string>)x.Values);
                }

                return result;
            }
        }
    }
}