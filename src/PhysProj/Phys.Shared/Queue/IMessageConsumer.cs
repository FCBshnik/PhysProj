namespace Phys.Shared.Queue
{
    public interface IMessageConsumer
    {
        void Consume(string message);
    }
}
