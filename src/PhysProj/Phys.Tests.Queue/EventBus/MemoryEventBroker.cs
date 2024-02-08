using Microsoft.Extensions.Logging;
using Phys.Shared.EventBus.Broker;
using System.Collections.Concurrent;

namespace Phys.Tests.Queue.EventBus
{
    internal class MemoryEventBroker : IEventBroker
    {
        private readonly ILogger<MemoryEventBroker> log;

        public MemoryEventBroker(ILogger<MemoryEventBroker> log)
        {
            this.log = log;
        }

        private readonly ConcurrentDictionary<string, HashSet<IEventBrokerHandler>> handlers =
            new ConcurrentDictionary<string, HashSet<IEventBrokerHandler>>();

        public void Publish(string eventName, ReadOnlyMemory<byte> eventData)
        {
            if (handlers.TryGetValue(eventName, out var eventHandlers))
            {
                eventHandlers.AsParallel().ForAll(h =>
                {
                    try
                    {
                        h.Handle(eventData);
                    }
                    catch (Exception e)
                    {
                        log.LogError(e, "failed to handle");
                    }
                });
            }
        }

        public IDisposable Subscribe(string eventName, IEventBrokerHandler handler)
        {
            var eventHandlers = handlers.GetOrAdd(eventName, _ => new HashSet<IEventBrokerHandler>());
            eventHandlers.Add(handler);
            return new Sub(eventHandlers, handler);
        }

        private class Sub : IDisposable
        {
            private readonly HashSet<IEventBrokerHandler> handlers;
            private readonly IEventBrokerHandler handler;

            public Sub(HashSet<IEventBrokerHandler> handlers, IEventBrokerHandler handler)
            {
                this.handlers = handlers;
                this.handler = handler;

                handlers.Add(handler);
            }

            public void Dispose()
            {
                handlers.Remove(handler);
            }
        }
    }
}
