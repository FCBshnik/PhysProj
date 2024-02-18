using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class AuthorDeletedEvent : IEvent
    {
        public string Name => EventNames.AuthorDeleted;

        public required string Code { get; set; }
    }
}
