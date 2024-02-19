using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Files.Events
{
    public class FileCreatedEvent : IEvent
    {
        public string Name => EventNames.FileCreated;

        public required string Code { get; set; }
    }
}
