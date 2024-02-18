using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class AuthorUpdatedEvent : IEvent
    {
        public string Name => EventNames.AuthorUpdated;

        public required string Code { get; set; }
    }
}
