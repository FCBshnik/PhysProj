using Microsoft.Extensions.Logging;
using Phys.Shared.Queue;
using System.Collections.Concurrent;

namespace Phys.Tests.Queue
{
    internal class MemoryQueue : IQueue
    {
        private readonly ConcurrentDictionary<string, IConsumer> consumers =
            new ConcurrentDictionary<string, IConsumer>(StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<MemoryQueue> log;

        public MemoryQueue(ILogger<MemoryQueue> log)
        {
            this.log = log;
        }

        public void Consume(string queue, IConsumer consumer)
        {
            consumers[queue] = consumer;
        }

        public void Publish(string queue, object message)
        {
            if (!consumers.TryGetValue(queue, out var consumer))
                throw new InvalidOperationException($"there is no consumer for queue '{queue}'");

            Task.Factory.StartNew(() => consumer.Consume(message))
                .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
