using MediatR;

namespace Katalye.Components.Notifications
{
    public static class MinionDiscovered
    {
        public class Notification : INotification
        {
            public string MinionSlug { get; set; }
        }
    }
}