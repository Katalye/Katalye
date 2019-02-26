using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Katalye.Api.Hubs;
using Katalye.Components.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NLog;

namespace Katalye.Api.Notifications.Handlers
{
    [UsedImplicitly]
    public class PublishEvent : INotificationHandler<EventOccured.Notification>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IHubContext<EventsHub> _hubContext;

        public PublishEvent(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(EventOccured.Notification notification, CancellationToken cancellationToken)
        {
            Logger.Info($"Publishing notification {notification.Path}.");
            await _hubContext.Clients.All.SendAsync("publish", notification.Path, notification.Data, cancellationToken);
        }
    }
}