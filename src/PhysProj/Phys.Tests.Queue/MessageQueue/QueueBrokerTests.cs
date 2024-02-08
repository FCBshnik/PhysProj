using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.NLog;
using System.Text;
using Phys.Shared.Queue.Broker;

namespace Phys.Tests.Queue.Queue
{
    public abstract class QueueBrokerTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        private IQueueBroker queue;

        protected abstract IQueueBroker CreateQueueBroker();

        public QueueBrokerTests()
        {
            NLogConfig.Configure(loggerFactory);
            queue = CreateQueueBroker();
        }

        [Fact]
        public void ConsumeDifferent_PublishToOne_ConsumedByOne()
        {
            var consumer1 = new GoodConsumer();
            using var unsub1 = queue.Consume("test-1", consumer1);
            var consumer2 = new GoodConsumer();
            using var unsub2 = queue.Consume("test-2", consumer2);

            queue.Send("test-2", "message");

            Thread.Sleep(100);
            consumer1.Consumed.Should().BeEquivalentTo(new List<object>());
            Thread.Sleep(100);
            consumer2.Consumed.Should().BeEquivalentTo(new List<string> { "message" });
        }

        [Fact]
        public void Consume_Publish_Consumed()
        {
            var consumer = new GoodConsumer();
            using var unsub = queue.Consume("test-1", consumer);

            Thread.Sleep(100);
            queue.Send("test-1", "message");

            Thread.Sleep(100);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message" });
        }

        [Fact]
        public void Consume_PublishMany_ConsumedWithOrder()
        {
            var consumer = new GoodConsumer();
            using var unsub = queue.Consume("test-1", consumer);

            Thread.Sleep(500);
            queue.Send("test-1", "message-1");
            Thread.Sleep(500);
            queue.Send("test-1", "message-2");

            Thread.Sleep(500);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message-1", "message-2" });
        }

        [Fact]
        public void Publish_Consume_Consumed()
        {
            var consumer = new GoodConsumer();
            queue.Send("test-1", "message");

            Thread.Sleep(1000);
            using var unsub = queue.Consume("test-1", consumer);

            Thread.Sleep(1000);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message" });
        }

        [Fact]
        public void Publish_ConsumeFailed_NotRedelivered()
        {
            var badConsumer = new BadConsumer();
            var consumer = new GoodConsumer();

            queue.Send("test-1", "message");
            Thread.Sleep(1000);
            var unsubBad = queue.Consume("test-1", badConsumer);

            Thread.Sleep(100);
            badConsumer.Failed.Count.Should().BeGreaterThan(0);
            unsubBad.Dispose();

            using var unsub = queue.Consume("test-1", consumer);
            Thread.Sleep(1000);
            consumer.Consumed.Should().BeEquivalentTo(new List<object>());
        }

        [Fact]
        public void PublishMany_Consume_ConsumedWithOrder()
        {
            var consumer = new GoodConsumer();
            queue.Send("test-1", "message-1");
            queue.Send("test-1", "message-2");

            Thread.Sleep(1000);
            using var unsub = queue.Consume("test-1", consumer);

            Thread.Sleep(1000);
            consumer.Consumed.Should().BeEquivalentTo(new List<object> { "message-1", "message-2" });
        }

        private class GoodConsumer : IQueueBrokerConsumer
        {
            public List<string> Consumed { get; } = new List<string>();

            public void Consume(ReadOnlyMemory<byte> message)
            {
                Thread.Sleep(10);
                Consumed.Add(Encoding.UTF8.GetString(message.Span));
            }
        }

        private class BadConsumer : IQueueBrokerConsumer
        {
            public List<string> Failed { get; } = new List<string>();

            public void Consume(ReadOnlyMemory<byte> message)
            {
                Thread.Sleep(10);
                Failed.Add(Encoding.UTF8.GetString(message.Span));
                throw new Exception($"failed to consume {message}");
            }
        }
    }
}