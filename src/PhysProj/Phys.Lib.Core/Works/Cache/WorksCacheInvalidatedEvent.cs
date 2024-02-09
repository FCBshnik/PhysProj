using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Cache
{
    public class WorksCacheInvalidatedEvent : IEvent
    {
        public string Name => EventNames.WorksCacheInvalidated;
    }
}
