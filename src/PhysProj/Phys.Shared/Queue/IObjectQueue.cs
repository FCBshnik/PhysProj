namespace Phys.Shared.Queue
{
    public interface IObjectQueue
    {
        void Publish<T>(string queueName, T message);

        IDisposable Consume<T>(string queueName, IObjectConsumer<T> consumer);
    }
}
