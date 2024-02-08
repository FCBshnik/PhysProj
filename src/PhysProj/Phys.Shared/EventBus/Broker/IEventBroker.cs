namespace Phys.Shared.EventBus.Broker
{
    public interface IEventBroker
    {
        void Publish(string eventName, ReadOnlyMemory<byte> eventData);

        IDisposable Subscribe(string eventName, IEventBrokerHandler handler);
    }
}
