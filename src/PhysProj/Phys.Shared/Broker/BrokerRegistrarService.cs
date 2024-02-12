using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Queue;
using Phys.Shared.EventBus;
using System.Collections.Concurrent;

namespace Phys.Shared.Broker
{
    public class BrokerRegistrarService : IHostedService
    {
        private readonly ILogger<BrokerRegistrarService> log;
        private readonly IMessageQueue queue;
        private readonly IEventBus bus;

        private readonly ConcurrentDictionary<Func<IDisposable>, IDisposable?> consumers = new ConcurrentDictionary<Func<IDisposable>, IDisposable?>();
        private readonly ConcurrentDictionary<Func<IDisposable>, IDisposable?> handlers = new ConcurrentDictionary<Func<IDisposable>, IDisposable?>();

        public BrokerRegistrarService(IMessageQueue queue, ILogger<BrokerRegistrarService> log, IEventBus bus)
        {
            this.queue = queue;
            this.log = log;
            this.bus = bus;
        }

        public void AddConsumer<TMessage>(IMessageConsumer<TMessage> consumer)
        {
            consumers.TryAdd(() => queue.Consume(consumer), null);
            log.LogInformation($"added message consumer {consumer.GetType()} for queue '{consumer.QueueName}'");
        }

        public void AddHandler<TEvent>(IEventHandler<TEvent> handler)
        {
            handlers.TryAdd(() => bus.Subscribe(handler), null);
            log.LogInformation($"added event handler {handler.GetType()} for event '{handler.EventName}'");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var consume in consumers.Keys.ToList())
                consumers[consume] = consume();

            foreach (var handle in handlers.Keys.ToList())
                handlers[handle] = handle();

            log.LogInformation("started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var sub in consumers.Values)
                sub?.Dispose();

            foreach (var sub in handlers.Values)
                sub?.Dispose();

            log.LogInformation("stopped");
            return Task.CompletedTask;
        }
    }
}
