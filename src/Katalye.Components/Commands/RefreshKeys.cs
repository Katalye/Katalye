using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Common;
using Katalye.Components.Configuration;
using MediatR;
using NLog;

namespace Katalye.Components.Commands
{
    public static class RefreshKeys
    {
        public class Command : IRequest<Result>
        {
        }

        public class Result
        {
        }

        [UsedImplicitly]
        public class Handler : IRequestHandler<Command, Result>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private readonly IMediator _mediator;
            private readonly IKatalyeConfiguration _configuration;

            public Handler(IMediator mediator, IKatalyeConfiguration configuration)
            {
                _mediator = mediator;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                Logger.Info("Requesting all known keys from master.");

                await _mediator.Send(new CreateJob.Command
                {
                    Client = JobClient.WheelAsync,
                    Function = SaltCommands.KeyListAll,
                    Username = _configuration.SaltApiServiceUsername,
                    Password = _configuration.SaltApiServicePassword
                }, cancellationToken);

                return new Result();
            }
        }
    }
}