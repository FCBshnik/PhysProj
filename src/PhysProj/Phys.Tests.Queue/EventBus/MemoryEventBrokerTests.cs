using Microsoft.Extensions.Logging;
using Phys.Shared.EventBus.Broker;

namespace Phys.Tests.Queue.EventBus
{
    public class MemoryEventBrokerTests : EventBrokerTests
    {
        private readonly MemoryEventBroker broker = new MemoryEventBroker(loggerFactory.CreateLogger<MemoryEventBroker>());

        protected override IEventBroker Broker => broker;
    }
}
