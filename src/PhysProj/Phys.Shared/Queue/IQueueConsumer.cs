namespace Phys.Queue
{
    public interface IQueueConsumer<TMessage>
    {
        string QueueName { get; }

        void Consume(TMessage message);
    }
}
