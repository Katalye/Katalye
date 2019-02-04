using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Katalye.Components.Queries.Tasks
{
    public static class GetTaskById
    {
        public class Query : IRequest<Result>
        {
            [Required]
            public Guid? Id { get; set; }
        }

        public class Result
        {
            public Guid Id { get; set; }

            public string Tag { get; set; }

            public Dictionary<string, string> Metadata { get; set; }

            public BackgroundTaskStatus Status { get; set; }

            public DateTimeOffset StartedOn { get; set; }
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
                var result = await _context.AdHocTasks
                                           .Where(x => x.Id == message.Id)
                                           .Select(x => new Result
                                           {
                                               Id = x.Id,
                                               Status = x.Status,
                                               Tag = x.Tag,
                                               Metadata = x.Metadata,
                                               StartedOn = x.StartedOn
                                           }).SingleOrDefaultAsync(cancellationToken);

                return result;
            }
        }
    }
}