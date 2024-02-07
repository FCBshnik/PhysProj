using Phys.Shared.Queue;

namespace Phys.Queue
{
    /// <summary>
    /// Distributed message queue
    /// </summary>
    public interface IQueue
    {
        void Send<TMessage>(TMessage message) where TMessage: IQueueMessage;

        IDisposable Consume<TMessage>(IQueueConsumer<TMessage> consumer);
    }
}
