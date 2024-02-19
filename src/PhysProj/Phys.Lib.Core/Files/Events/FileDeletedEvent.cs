using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Files.Events
{
    public class FileDeletedEvent : IEvent
    {
        public string Name => EventNames.FileDeleted;

        public required string Code { get; set; }
    }
}
