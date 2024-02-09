using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class WorkCreatedEvent : IEvent
    {
        public string Name => EventNames.WorkCreated;

        public required string Code { get; set; }
    }
}
