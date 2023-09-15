namespace Phys.Shared.Queue
{
    public interface IConsumer
    {
        void Consume(object message);
    }
}
