using FluentAssertions;
using Microsoft.Extensions.Logging;
using Phys.NLog;
using Phys.Shared.EventBus.Broker;
using System.Text;

namespace Phys.Tests.Queue.EventBus
{
    public abstract class EventBrokerTests
    {
        protected static readonly LoggerFactory loggerFactory = new LoggerFactory();

        protected abstract IEventBroker Broker { get; }

        protected EventBrokerTests()
        {
            NLogConfig.Configure(loggerFactory);
        }

        [Fact]
        public void Subscribe_Publish_Handled()
        {
            var handler = new GoodHandler();
            using var unsub = Broker.Subscribe("test-event-1", handler);
            Thread.Sleep(100);

            Broker.Publish("test-event-1", "data");
            Thread.Sleep(100);

            handler.Handled.Should().BeEquivalentTo(new List<object> { "data" });
        }

        [Fact]
        public void Subscribe_PublishMany_HandledAll()
        {
            var handler = new GoodHandler();
            using var unsub = Broker.Subscribe("test-event-1", handler);
            Thread.Sleep(100);

            Broker.Publish("test-event-1", "data-1");
            Thread.Sleep(100);
            Broker.Publish("test-event-1", "data-2");
            Thread.Sleep(100);

            handler.Handled.Should().BeEquivalentTo(new List<object> { "data-1", "data-2" });
        }

        [Fact]
        public void SubscribeDifferent_PublishOne_HandledByOne()
        {
            var handler1 = new GoodHandler();
            var handler2 = new GoodHandler();
            using var unsub1 = Broker.Subscribe("test-event-1", handler1);
            using var unsub2 = Broker.Subscribe("test-event-2", handler2);
            Thread.Sleep(100);

            Broker.Publish("test-event-2", "data");
            Thread.Sleep(100);

            handler1.Handled.Should().BeEquivalentTo(new List<object>());
            handler2.Handled.Should().BeEquivalentTo(new List<object> { "data" });
        }

        [Fact]
        public void SubscribeDifferent_PublishDifferent_Handled()
        {
            var handler1 = new GoodHandler();
            var handler2 = new GoodHandler();
            using var unsub1 = Broker.Subscribe("test-event-1", handler1);
            using var unsub2 = Broker.Subscribe("test-event-2", handler2);
            Thread.Sleep(100);

            Broker.Publish("test-event-1", "data-1");
            Thread.Sleep(100);
            Broker.Publish("test-event-2", "data-2");
            Thread.Sleep(100);

            handler1.Handled.Should().BeEquivalentTo(new List<object> { "data-1" });
            handler2.Handled.Should().BeEquivalentTo(new List<object> { "data-2" });
        }

        [Fact]
        public void SubscribeMany_Publish_HandledByAll()
        {
            var handler1 = new GoodHandler("handler-1");
            var handler2 = new GoodHandler("handler-2");
            using var unsub1 = Broker.Subscribe("test-event-1", handler1);
            using var unsub2 = Broker.Subscribe("test-event-1", handler2);
            Thread.Sleep(100);

            Broker.Publish("test-event-1", "data");
            Thread.Sleep(100);

            handler1.Handled.Should().BeEquivalentTo(new List<object> { "data" });
            handler2.Handled.Should().BeEquivalentTo(new List<object> { "data" });
        }

        [Fact]
        public void Publish_Subscribe_NotHandled()
        {
            Broker.Publish("test-event-1", "data");
            Thread.Sleep(100);

            var handler = new GoodHandler();
            using var unsub = Broker.Subscribe("test-event-1", handler);
            Thread.Sleep(100);

            handler.Handled.Should().BeEquivalentTo(new List<object>());
        }

        [Fact]
        public void SubscribeBad_Publish_NotHandled()
        {
            var badHandler = new BadHandler();
            var badSub = Broker.Subscribe("test-event", badHandler);
            Thread.Sleep(100);

            Broker.Publish("test-event", "data");
            Thread.Sleep(100);

            badSub.Dispose();
            var goodHandler = new GoodHandler();
            using var goodSub = Broker.Subscribe("test-event", goodHandler);
            Thread.Sleep(100);

            goodHandler.Handled.Should().BeEquivalentTo(new List<object>());
        }

        private class GoodHandler : IEventBrokerHandler
        {
            public List<string> Handled { get; } = new List<string>();

            public string Name { get; }

            public GoodHandler(string name = "handler")
            {
                Name = name;
            }

            public void Handle(ReadOnlyMemory<byte> eventData)
            {
                Thread.Sleep(10);
                Handled.Add(Encoding.UTF8.GetString(eventData.Span));
            }
        }

        private class BadHandler : IEventBrokerHandler
        {
            public string Name { get; }

            public BadHandler(string name = "handler")
            {
                Name = name;
            }

            public void Handle(ReadOnlyMemory<byte> eventData)
            {
                Thread.Sleep(10);
                throw new Exception("failed to hanlde");
            }
        }
    }
}
