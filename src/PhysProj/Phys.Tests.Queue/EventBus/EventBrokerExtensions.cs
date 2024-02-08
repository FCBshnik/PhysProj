using Phys.Shared.EventBus.Broker;
using System.Text;

namespace Phys.Tests.Queue.EventBus
{
    internal static class EventBrokerExtensions
    {
        public static void Publish(this IEventBroker eventBroker, string eventName, string eventData)
        {
            eventBroker.Publish(eventName, Encoding.UTF8.GetBytes(eventData));
        }
    }
}
