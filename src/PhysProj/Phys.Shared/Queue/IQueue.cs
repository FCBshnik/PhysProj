namespace Phys.Shared.Queue
{
    public interface IQueue
    {
        void Publish(string queue, object message);

        void Consume(string queue, IConsumer consumer);
    }
}
