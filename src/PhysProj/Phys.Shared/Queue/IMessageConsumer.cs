namespace Phys.Queue
{
    public interface IMessageConsumer<TMessage>
    {
        string QueueName { get; }

        void Consume(TMessage message);
    }
}
