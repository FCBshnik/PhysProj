namespace Phys.Queue
{
    public interface IConsumer<T>
    {
        void Consume(T message);
    }
}
