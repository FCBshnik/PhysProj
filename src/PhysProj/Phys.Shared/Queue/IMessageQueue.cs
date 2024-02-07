namespace Phys.Queue
{
    public interface IMessageQueue
    {
        void Publish(string queueName, ReadOnlyMemory<byte> message);

        IDisposable Consume(string queueName, IMessageConsumer consumer);
    }
}
