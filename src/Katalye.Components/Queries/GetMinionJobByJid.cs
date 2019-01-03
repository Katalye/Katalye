using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Katalye.Components.Queries
{
    public static class GetMinionJobByJid
    {
        public class Query : IRequest<Result>
        {
            [Required]
            public string Id { get; set; }

            [Required]
            public string Jid { get; set; }
        }

        public class Result
        {
            public string Jid { get; set; }

            public string Function { get; set; }

            public JArray Arguments { get; set; }

            public JObject ReturnData { get; set; }

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
                                     .Where(x => x.Minion.MinionSlug == message.Id && x.Job.Jid == message.Jid)
                                     .Select(x => new Result
                                     {
                                         Jid = x.Job.Jid,
                                         Function = x.Job.Function,
                                         Success = x.Success,
                                         ReturnedOn = x.Timestamp,
                                         Arguments = x.Job.Arguments,
                                         ReturnData = x.ReturnData
                                     })
                                     .SingleOrDefaultAsync(cancellationToken);
            }
        }
    }
}