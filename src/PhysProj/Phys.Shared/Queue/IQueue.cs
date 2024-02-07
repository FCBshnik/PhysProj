using Phys.Shared.Queue;

namespace Phys.Queue
{
    public interface IQueue
    {
        void Publish<TMessage>(TMessage message) where TMessage: IQueueMessage;

        IDisposable Consume<TMessage>(IQueueConsumer<TMessage> consumer);
    }
}
