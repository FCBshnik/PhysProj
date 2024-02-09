namespace Phys.Shared.EventBus
{
    /// <summary>
    /// Distributed event bus
    /// </summary>
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent data) where TEvent : IEvent;

        IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler);
    }
}