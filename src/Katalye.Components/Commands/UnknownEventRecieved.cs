using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Data;
using Katalye.Data.Entities;
using MediatR;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Components.Commands
{
    public static class UnknownEventRecieved
    {
        public class Command : IRequest<Result>
        {
            public string Tag { get; set; }

            public JObject Data { get; set; }
        }

        public class Result
        {
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
                Logger.Warn($"Got unknown tag {message.Tag} with {message.Data}.");

                _context.UnknownEvents.Add(new UnknownEvent
                {
                    Data = message.Data,
                    Tag = message.Tag
                });
                await _context.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}