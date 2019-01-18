using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Components.Commands;
using MediatR;
using NLog;

namespace Katalye.Components.Notifications.Handlers
{
    [UsedImplicitly]
    public class RefreshDiscoveredMinion : INotificationHandler<MinionDiscovered.Notification>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public RefreshDiscoveredMinion(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(MinionDiscovered.Notification notification, CancellationToken cancellationToken)
        {
            Logger.Info($"Minion {notification.MinionSlug} was newly discovered. Will refresh grains.");

            await _mediator.Send(new RefreshMinionGrains.Command
            {
                Minions = new List<string>
                {
                    notification.MinionSlug
                }
            }, cancellationToken);
        }
    }
}