using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Events
{
    public class WorkUpdatedEvent : IEvent
    {
        public string Name => EventNames.WorkUpdated;

        public required string Code { get; set; }
    }
}
