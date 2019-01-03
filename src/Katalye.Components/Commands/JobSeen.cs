using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Components.Commands
{
    public static class JobSeen
    {
        public class Command : IRequest<Result>
        {
            public string Jid { get; set; }
            public string Function { get; set; }
            public JArray Arguments { get; set; }
        }

        public class Result
        {
            public Guid JobId { get; set; }
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly KatalyeContext _context;

            public Handler(KatalyeContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                using (var unit = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    var jobId = await _context.Jobs
                                              .Where(x => x.Jid == message.Jid)
                                              .Select(x => (Guid?) x.Id)
                                              .SingleOrDefaultAsync(cancellationToken);

                    var jobExists = jobId != null;
                    if (!jobExists)
                    {
                        Logger.Trace($"Job with a jid {message.Jid} has never been seen, it will be recorded.");

                        var job = new Job
                        {
                            Jid = message.Jid,
                            Arguments = message.Arguments,
                            Function = message.Function
                        };
                        _context.Jobs.Add(job);
                        await _context.SaveChangesAsync(cancellationToken);

                        jobId = job.Id;
                    }

                    unit.Commit();

                    return new Result
                    {
                        JobId = jobId.Value
                    };
                }
            }
        }
    }
}