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

namespace Katalye.Components.Queries.Tasks
{
    public static class GetTasks
    {
        public class Query : IRequest<Result>, IPaginatedQuery
        {
            [Required]
            public Guid? Id { get; set; }

            [Required]
            public ICollection<BackgroundTaskStatus> Status { get; set; } = new List<BackgroundTaskStatus>
            {
                BackgroundTaskStatus.Processing,
                BackgroundTaskStatus.Queued
            };

            public int? Page { get; set; }

            public int? Size { get; set; }
        }

        public class Result : PagedResult<Model>
        {
        }

        public class Model
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
                                           .Where(x => message.Status.Contains(x.Status))
                                           .OrderByDescending(x => x.StartedOn)
                                           .Select(x => new Model
                                           {
                                               Id = x.Id,
                                               Status = x.Status,
                                               Tag = x.Tag,
                                               Metadata = x.Metadata,
                                               StartedOn = x.StartedOn
                                           }).PageAsync(message, new Result());

                return result;
            }
        }
    }
}