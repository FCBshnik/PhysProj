using Phys.Shared.Queue;

namespace Phys.Queue
{
    /// <summary>
    /// Distributed message queue
    /// </summary>
    public interface IMessageQueue
    {
        void Send<TMessage>(TMessage message) where TMessage: IMessage;

        IDisposable Consume<TMessage>(IMessageConsumer<TMessage> consumer);
    }
}
