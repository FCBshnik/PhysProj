namespace Phys.Shared.EventBus.Broker
{
    /// <summary>
    /// Transaport interface for distributed event bus
    /// </summary>
    public interface IEventBroker
    {
        void Publish(string eventName, ReadOnlyMemory<byte> eventData);

        IDisposable Subscribe(string eventName, IEventBrokerHandler handler);
    }
}
