using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.NLog;
using Phys.Queue;

namespace Phys.Tests.Queue
{
    public abstract class QueueTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        private IMessageQueue queue;

        protected abstract IMessageQueue CreateQueue();

        public QueueTests()
        {
            NLogConfig.Configure(loggerFactory, "tests-queues");
            queue = CreateQueue();
        }

        [Fact]
        public void ConsumeDifferent_PublishToOne_ConsumedByOne()
        {
            var consumer1 = new Consumer();
            using var unsub1 = queue.Consume("test-1", consumer1);
            var consumer2 = new Consumer();
            using var unsub2 = queue.Consume("test-2", consumer2);

            queue.Publish("test-2", "message");
            Thread.Sleep(100);
            consumer1.Consumed.Should().BeEquivalentTo(new List<object>());
            Thread.Sleep(100);
            consumer2.Consumed.Should().BeEquivalentTo(new List<string> { "message" });
        }

        [Fact]
        public void Consume_Publish_Consumed()
        {
            var consumer = new Consumer();
            using var unsub = queue.Consume("test-1", consumer);
            Thread.Sleep(100);
            queue.Publish("test-1", "message");
            Thread.Sleep(100);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message" });
        }

        [Fact]
        public void Consume_PublishMany_Consumed()
        {
            var consumer = new Consumer();
            using var unsub = queue.Consume("test-1", consumer);
            Thread.Sleep(500);
            queue.Publish("test-1", "message-1");
            Thread.Sleep(500);
            queue.Publish("test-1", "message-2");
            Thread.Sleep(500);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message-1", "message-2" });
        }

        [Fact]
        public void Publish_Consume_Consumed()
        {
            var consumer = new Consumer();
            queue.Publish("test-1", "message");
            Thread.Sleep(1000);
            using var unsub = queue.Consume("test-1", consumer);
            Thread.Sleep(1000);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message" });
        }

        [Fact]
        public void PublishMany_Consume_Consumed()
        {
            var consumer = new Consumer();
            queue.Publish("test-1", "message-1");
            queue.Publish("test-1", "message-2");
            Thread.Sleep(1000);
            using var unsub = queue.Consume("test-1", consumer);
            Thread.Sleep(1000);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message-1", "message-2" });
        }

        private class Consumer : IMessageConsumer
        {
            public List<string> Consumed { get; } = new List<string>();

            public void Consume(string message)
            {
                Thread.Sleep(10);
                Consumed.Add(message);
            }
        }
    }
}