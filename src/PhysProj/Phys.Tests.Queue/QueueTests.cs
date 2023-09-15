using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.Shared.Logging;
using Phys.Shared.NLog;
using Phys.Shared.Queue;

namespace Phys.Tests.Queue
{
    public class QueueTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        private IQueue queue = new MemoryQueue(loggerFactory.CreateLogger<MemoryQueue>());

        public QueueTests()
        {
            NLogConfig.Configure(loggerFactory, "queue-tests");
        }

        [Fact]
        public void Test()
        {
            var consumer = new Consumer();
            queue.Consume("test-1", consumer);
            queue.Publish("test-1", "message");
            Thread.Sleep(100);
            consumer.Consumed.Should().Be("message");
        }

        private class Consumer : IConsumer
        {
            public object? Consumed { get; set; }

            public void Consume(object message)
            {
                Consumed = message;
            }
        }
    }
}