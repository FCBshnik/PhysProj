using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Files.Cache
{
    public class FilesCacheInvalidatedEvent : IEvent
    {
        public string Name => EventNames.FilesCacheInvalidated;
    }
}
