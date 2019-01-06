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
    public static class GetSearchGrainValues
    {
        public class Query : IRequest<Result>
        {
            [Required]
            public string Path { get; set; }

            public string Search { get; set; }
        }

        public class Result
        {
            public ICollection<string> Values { get; set; }
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
                var noSearch = string.IsNullOrWhiteSpace(message.Search);
                var likeSearch = $"%{message.Search}%";
                var values = await _context.MinionGrainValues
                                           .Where(x => x.MinionGrain.Path == message.Path)
                                           .Select(x => x.Value)
                                           .Where(x => noSearch || EF.Functions.ILike(x, likeSearch))
                                           .Distinct()
                                           .Take(10)
                                           .ToListAsync(cancellationToken);

                return new Result
                {
                    Values = values
                };
            }
        }
    }
}