using System;
using MediatR;

namespace Katalye.Components.Notifications
{
    public static class JobReturnSaved
    {
        public class Notification : INotification
        {
            public Guid MinionReturnEventId { get; set; }

            public string Function { get; set; }
        }
    }
}