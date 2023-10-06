namespace Phys.Queue
{
    public interface IMessageConsumer
    {
        void Consume(string message);
    }
}
