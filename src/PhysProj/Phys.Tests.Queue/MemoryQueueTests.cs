using Microsoft.Extensions.Logging;
using Phys.Shared.Queue;

namespace Phys.Tests.Queue
{
    public class MemoryQueueTests : QueueTests
    {
        protected override IMessageQueue CreateQueue() => new MemoryQueue(loggerFactory.CreateLogger<MemoryQueue>());
    }
}
