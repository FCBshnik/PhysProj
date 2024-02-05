namespace Phys.Queue
{
    public interface IQueue
    {
        void Publish<T>(string queueName, T message);

        IDisposable Consume<T>(string queueName, IConsumer<T> consumer);
    }
}
