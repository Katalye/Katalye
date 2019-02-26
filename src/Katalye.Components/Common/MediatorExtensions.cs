using System.Linq;
using System.Threading.Tasks;
using Katalye.Components.Notifications;
using MediatR;

namespace Katalye.Components.Common
{
    public static class MediatorExtensions
    {
        public static Task PublishEvent(this IMediator mediator, string path, params (string Key, string Value)[] keyValues)
        {
            return mediator.Publish(new EventOccured.Notification
            {
                Path = path,
                Data = keyValues.ToDictionary(x => x.Key, x => x.Value)
            });
        }
    }
}