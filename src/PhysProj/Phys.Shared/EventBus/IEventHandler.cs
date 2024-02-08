namespace Phys.Shared.EventBus
{
    public interface IEventHandler<TEvent>
    {
        string EventName { get; }

        void Handle(TEvent data);
    }
}