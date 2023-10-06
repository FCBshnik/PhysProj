namespace Phys.Queue
{
    public interface IMessageQueue
    {
        void Publish(string queueName, string message);

        IDisposable Consume(string queueName, IMessageConsumer consumer);
    }
}
