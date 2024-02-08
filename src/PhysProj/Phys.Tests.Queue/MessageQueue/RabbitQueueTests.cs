using Microsoft.Extensions.Logging;
using Phys.Queue;
using Phys.RabbitMQ;
using RabbitMQ.Client;
using Phys.Shared.Queue.Broker;

namespace Phys.Tests.Queue.Queue
{
    public class RabbitQueueTests : QueueBrokerTests
    {
        protected override IQueueBroker CreateQueueBroker() => new RabbitQueueBroker(new ConnectionFactory { HostName = "192.168.2.67" }, "physlib-tests", loggerFactory.CreateLogger<RabbitQueueBroker>());
    }
}
