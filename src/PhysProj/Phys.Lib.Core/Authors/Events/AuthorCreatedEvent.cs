using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class AuthorCreatedEvent : IEvent
    {
        public string Name => EventNames.AuthorCreated;

        public required string Code { get; set; }
    }
}
