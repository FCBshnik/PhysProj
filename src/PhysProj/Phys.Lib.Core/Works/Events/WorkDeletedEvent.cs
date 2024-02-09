using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class WorkDeletedEvent : IEvent
    {
        public string Name => EventNames.WorkDeleted;

        public required string Code { get; set; }
    }
}
