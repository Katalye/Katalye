using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Commands;
using MediatR;

namespace Katalye.Components.Processing
{
    [UsedImplicitly]
    public class GrainsRefreshProcessingServer : ProcessingServer
    {
        public override TimeSpan Interval { get; } = TimeSpan.FromHours(24);

        private readonly IMediator _mediator;

        public GrainsRefreshProcessingServer(IMediator mediator, IKatalyeConfiguration configuration) : base(configuration)
        {
            _mediator = mediator;
        }

        public override async Task Process(CancellationToken cancellationToken)
        {
            await _mediator.Send(new RefreshMinionGrains.Command
            {
                Age = TimeSpan.FromHours(12)
            }, cancellationToken);
        }
    }
}