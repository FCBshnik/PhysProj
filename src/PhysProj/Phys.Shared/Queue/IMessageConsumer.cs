namespace Phys.Queue
{
    public interface IMessageConsumer
    {
        void Consume(ReadOnlyMemory<byte> message);
    }
}
