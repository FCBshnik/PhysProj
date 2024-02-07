namespace Phys.Queue
{
    public interface IQueue
    {
        void Publish<TMessage>(string queueName, TMessage message);

        IDisposable Consume<TMessage>(IQueueConsumer<TMessage> consumer);
    }
}
