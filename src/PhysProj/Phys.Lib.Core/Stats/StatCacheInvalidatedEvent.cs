using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Stats
{
    public class StatCacheInvalidatedEvent : IEvent
    {
        public string Name => EventNames.StatCacheInvalidated;
    }
}
