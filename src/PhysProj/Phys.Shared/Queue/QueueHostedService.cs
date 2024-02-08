using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Queue;
using System.Collections.Concurrent;

namespace Phys.Shared.Queue
{
    public class QueueHostedService : IHostedService
    {
        private readonly IMessageQueue queue;
        private readonly ILogger<QueueHostedService> log;

        private readonly ConcurrentDictionary<Func<IDisposable>, IDisposable?> subs = new ConcurrentDictionary<Func<IDisposable>, IDisposable?>();

        public QueueHostedService(IMessageQueue queue, ILogger<QueueHostedService> log)
        {
            this.queue = queue;
            this.log = log;
        }

        public void AddConsumer<TMessage>(IMessageQueueConsumer<TMessage> consumer)
        {
            subs.TryAdd(() => queue.Consume(consumer), null);
            log.LogInformation($"added consumer {consumer.GetType()} with queue '{consumer.QueueName}'");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var consume in subs.Keys.ToList())
                subs[consume] = consume();

            log.LogInformation("started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var sub in subs.Values)
                sub?.Dispose();

            log.LogInformation("stopped");
            return Task.CompletedTask;
        }
    }
}
