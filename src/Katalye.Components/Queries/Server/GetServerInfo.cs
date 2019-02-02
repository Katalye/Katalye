using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components.Queries.Server
{
    public static class GetServerInfo
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public string ApiVersion { get; set; }

            public IList<string> AppliedMigrations { get; set; }
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
                var version = Assembly.GetEntryAssembly().ImageRuntimeVersion;
                var migrations = (await _context.Database.GetAppliedMigrationsAsync()).ToList();

                var result = new Result
                {
                    AppliedMigrations = migrations,
                    ApiVersion = version
                };

                return result;
            }
        }
    }
}