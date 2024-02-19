using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Files.Events
{
    public class FileUpdatedEvent : IEvent
    {
        public string Name => EventNames.FileUpdated;

        public required string Code { get; set; }
    }
}
