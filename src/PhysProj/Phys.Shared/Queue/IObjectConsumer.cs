namespace Phys.Queue
{
    public interface IObjectConsumer<T>
    {
        void Consume(T message);
    }
}
