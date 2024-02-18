using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Authors.Cache
{
    public class AuthorsCacheInvalidatedEvent : IEvent
    {
        public string Name => EventNames.AuthorsCacheInvalidated;
    }
}
