using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Newtonsoft.Json.Linq;

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

            public JArray Arguments { get; set; }

            public string User { get; set; }

            public IList<string> Targets { get; set; }

            public string TargetType { get; set; }

            public DateTimeOffset? CreatedOn { get; set; }

            public bool CreationEventExists { get; set; }
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
                var result = await (from returnEvent in _context.MinionReturnEvents.Where(x => x.Minion.MinionSlug == message.Id)
                                    from creationEvent in _context.JobCreationEvents.Where(x => x.JobId == returnEvent.JobId).DefaultIfEmpty()
                                    let job = returnEvent.Job
                                    orderby returnEvent.Timestamp descending
                                    select new Model
                                    {
                                        Jid = job.Jid,
                                        Function = job.Function,
                                        Arguments = job.Arguments,
                                        Success = returnEvent.Success,
                                        ReturnedOn = returnEvent.Timestamp,
                                        TargetType = creationEvent.TargetType,
                                        User = creationEvent.User,
                                        Targets = creationEvent.Targets,
                                        CreationEventExists = creationEvent != null,
                                        CreatedOn = creationEvent.Timestamp
                                    }).PageAsync(message, new Result());

                return result;
            }
        }
    }
}