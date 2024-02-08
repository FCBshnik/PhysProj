namespace Phys.Shared.Queue.Broker
{
    /// <summary>
    /// Transaport interface for distributed message queue
    /// </summary>
    public interface IQueueBroker
    {
        void Send(string queueName, ReadOnlyMemory<byte> message);

        IDisposable Consume(string queueName, IQueueBrokerConsumer consumer);
    }
}
