using Microsoft.Extensions.Logging;
using Phys.Queue;
using System.Collections.Concurrent;

namespace Phys.Tests.Queue
{
    internal class MemoryQueue : IMessageQueue
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, Subscription>> subscriptions =
            new ConcurrentDictionary<string, ConcurrentDictionary<int, Subscription>>();

        private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> messages =
            new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        private readonly ILogger<MemoryQueue> log;

        public MemoryQueue(ILogger<MemoryQueue> log)
        {
            this.log = log;
        }

        public IDisposable Consume(string queueName, IMessageConsumer consumer)
        {
            var subs = subscriptions.GetOrAdd(queueName, _ => new ConcurrentDictionary<int, Subscription>());
            var key = consumer.GetHashCode();
            var sub = new Subscription(() => subs.Remove(key, out _), consumer, log);
            subs.TryAdd(key, sub);
            ConsumeAsync(queueName);
            return sub;
        }

        public void Publish(string queueName, string message)
        {
            var msgs = messages.GetOrAdd(queueName, _ => new ConcurrentQueue<string>());
            msgs.Enqueue(message);
            ConsumeAsync(queueName);
        }

        private Task ConsumeAsync(string queueName)
        {
            return Task.Factory.StartNew(() => Consume(queueName));
        }

        private void Consume(string queue)
        {
            if (!subscriptions.TryGetValue(queue, out var subs))
                return;

            var sub = subs.Values.FirstOrDefault(s => !s.IsBusy);
            if (sub == null || !messages.TryGetValue(queue, out var msgs))
                return;

            if (!msgs.TryDequeue(out var msg))
                return;

            // do not redeliver
            sub.Consume(msg);
            //if (!sub.Consume(msg))
            ///    msgs.Enqueue(msg);

            if (!msgs.IsEmpty)
                Consume(queue);
        }

        private class Subscription : IDisposable
        {
            private readonly Action dispose;
            private readonly IMessageConsumer consumer;
            private readonly ILogger<MemoryQueue> log;

            public bool IsBusy { get; private set; }

            public Subscription(Action dispose, IMessageConsumer consumer, ILogger<MemoryQueue> log)
            {
                this.dispose = dispose;
                this.consumer = consumer;
                this.log = log;
            }

            public bool Consume(string message)
            {
                try
                {
                    IsBusy = true;
                    consumer.Consume(message);
                    return true;
                }
                catch (Exception e)
                {
                    log.LogError(e, $"failed consume '{message}'");
                }
                finally
                {
                    IsBusy = false;
                }

                return false;
            }

            public void Dispose()
            {
                dispose();
            }
        }
    }
}
