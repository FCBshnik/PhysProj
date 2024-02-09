using Microsoft.Extensions.Logging;
using System.Text.Json;
using Phys.Shared.EventBus;
using Phys.Shared.Queue.Broker;
using Phys.Queue;
using Phys.Shared.EventBus.Broker;

namespace Phys.Shared.Broker
{
    /// <summary>
    /// JSON (de)serialization wrapper over transport brokers
    /// </summary>
    public class JsonMessageBroker : IMessageQueue, IEventBus
    {
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString | System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        };

        private readonly ILogger<JsonMessageBroker> log;
        private readonly IQueueBroker queueBroker;
        private readonly IEventBroker eventsBroker;

        public JsonMessageBroker(IQueueBroker queueBroker, IEventBroker eventsBroker, ILogger<JsonMessageBroker> log)
        {
            this.queueBroker = queueBroker;
            this.eventsBroker = eventsBroker;
            this.log = log;
        }

        IDisposable IMessageQueue.Consume<T>(IMessageConsumer<T> consumer)
        {
            ArgumentNullException.ThrowIfNull(consumer);

            return queueBroker.Consume(consumer.QueueName, new QueueConsumer<T>(consumer, log));
        }

        void IMessageQueue.Send<T>(T message)
        {
            ArgumentNullException.ThrowIfNull(message);

            queueBroker.Send(message.QueueName, JsonSerializer.SerializeToUtf8Bytes(message, serializerOptions));
        }

        void IEventBus.Publish<TEvent>(TEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            eventsBroker.Publish(@event.Name, JsonSerializer.SerializeToUtf8Bytes(@event, serializerOptions));
        }

        IDisposable IEventBus.Subscribe<TEvent>(IEventHandler<TEvent> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            return eventsBroker.Subscribe(handler.EventName, new EventHandler<TEvent>(log, handler));
        }

        private class EventHandler<T> : IEventBrokerHandler
        {
            private readonly ILogger<JsonMessageBroker> log;
            private readonly IEventHandler<T> handler;

            public EventHandler(ILogger<JsonMessageBroker> log, IEventHandler<T> handler)
            {
                this.log = log;
                this.handler = handler;
            }

            public string Name => handler.EventName;

            void IEventBrokerHandler.Handle(ReadOnlyMemory<byte> eventData)
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<T>(eventData.Span, serializerOptions)!;
                    handler.Handle(obj);
                }
                catch (Exception e)
                {
                    log.LogError(e, $"failed handle {typeof(T)}");
                }
            }
        }

        private class QueueConsumer<T> : IQueueBrokerConsumer
        {
            private readonly ILogger<JsonMessageBroker> log;
            private readonly IMessageConsumer<T> consumer;

            public QueueConsumer(IMessageConsumer<T> consumer, ILogger<JsonMessageBroker> log)
            {
                this.consumer = consumer;
                this.log = log;
            }

            void IQueueBrokerConsumer.Consume(ReadOnlyMemory<byte> message)
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<T>(message.Span, serializerOptions)!;
                    consumer.Consume(obj);
                }
                catch (Exception e)
                {
                    log.LogError(e, $"failed consume {typeof(T)}");
                }
            }
        }
    }
}
