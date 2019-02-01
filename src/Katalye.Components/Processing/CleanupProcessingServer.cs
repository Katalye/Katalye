using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Commands;
using Katalye.Components.Configuration;
using MediatR;

namespace Katalye.Components.Processing
{
    [UsedImplicitly]
    public class CleanupProcessingServer : ProcessingServer
    {
        private readonly IMediator _mediator;

        public override TimeSpan Interval { get; } = TimeSpan.FromHours(1);

        public CleanupProcessingServer(IMediator mediator, IKatalyeConfiguration configuration) : base(configuration)
        {
            _mediator = mediator;
        }

        public override async Task Process(CancellationToken cancellationToken)
        {
            await _mediator.Send(new CleanupGrainGenerations.Command(), cancellationToken);
        }
    }
}