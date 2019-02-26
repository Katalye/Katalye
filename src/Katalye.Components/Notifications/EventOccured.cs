using System.Collections.Generic;
using MediatR;

namespace Katalye.Components.Notifications
{
    public static class EventOccured
    {
        public class Notification : INotification
        {
            public string Path { get; set; }

            public IDictionary<string, string> Data { get; set; }
        }
    }
}