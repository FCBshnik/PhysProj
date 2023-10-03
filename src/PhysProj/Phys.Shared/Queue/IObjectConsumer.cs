namespace Phys.Shared.Queue
{
    public interface IObjectConsumer<T>
    {
        void Consume(T message);
    }
}
