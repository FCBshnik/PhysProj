using Microsoft.Extensions.Logging;
using Phys.RabbitMQ;
using Phys.Shared.EventBus.Broker;
using RabbitMQ.Client;

namespace Phys.Tests.Queue.EventBus
{
    public class RabbitEventBrokerTests : EventBrokerTests
    {
        private readonly RabbitEventBroker broker = new RabbitEventBroker(new ConnectionFactory { HostName = "192.168.2.67" }, "physlib-tests.events", loggerFactory.CreateLogger<RabbitEventBroker>());

        protected override IEventBroker Broker => broker;
    }
}
