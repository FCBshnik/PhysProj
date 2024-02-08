namespace Phys.Shared.Queue.Broker
{
    public interface IQueueBrokerConsumer
    {
        void Consume(ReadOnlyMemory<byte> message);
    }
}
