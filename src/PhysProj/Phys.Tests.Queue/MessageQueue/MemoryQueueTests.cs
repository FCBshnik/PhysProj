using Microsoft.Extensions.Logging;
using Phys.Queue;
using Phys.Shared.Queue.Broker;

namespace Phys.Tests.Queue.Queue
{
    public class MemoryQueueTests : QueueBrokerTests
    {
        protected override IQueueBroker CreateQueueBroker() => new MemoryQueue(loggerFactory.CreateLogger<MemoryQueue>());
    }
}
