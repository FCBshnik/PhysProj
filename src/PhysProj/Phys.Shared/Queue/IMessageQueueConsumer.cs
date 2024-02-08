using Phys.Shared.Queue;

namespace Phys.Queue
{
    public interface IMessageQueueConsumer<TMessage>
    {
        string QueueName { get; }

        void Consume(TMessage message);
    }
}
