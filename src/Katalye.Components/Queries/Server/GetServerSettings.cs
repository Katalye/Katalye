using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components.Queries.Server
{
    public static class GetServerSettings
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public Dictionary<string, Setting> Settings { get; set; }
        }

        public class Setting
        {
            public string Key { get; set; }

            public string Value { get; set; }

            public int Version { get; set; }

            public DateTimeOffset? LastUpdated { get; set; }
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
                var settings = await (from setting in _context.ServerSettings
                                      select new Setting
                                      {
                                          Value = setting.Value,
                                          Key = setting.Key,
                                          Version = setting.Version,
                                          LastUpdated = setting.LastUpdated
                                      }).ToListAsync(cancellationToken);

                return new Result
                {
                    Settings = settings.ToDictionary(x => x.Key, x => x)
                };
            }
        }
    }
}