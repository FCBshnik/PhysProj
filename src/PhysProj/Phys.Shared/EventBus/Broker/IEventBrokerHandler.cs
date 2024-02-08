namespace Phys.Shared.EventBus.Broker
{
    public interface IEventBrokerHandler
    {
        string Name { get; }

        void Handle(ReadOnlyMemory<byte> eventData);
    }
}
