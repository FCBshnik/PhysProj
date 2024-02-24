using Microsoft.Extensions.Logging;
using Phys.Shared.EventBus.Broker;
using RabbitMQ.Client;
using System.Reflection;

namespace Phys.RabbitMQ
{
    public class RabbitEventBroker : IEventBroker
    {
        private readonly ILogger<RabbitEventBroker> log;
        private readonly Lazy<IConnection> connection;
        private readonly string prefix;

        public RabbitEventBroker(IConnectionFactory connectionFactory, string prefix, ILogger<RabbitEventBroker> log)
        {
            this.log = log;
            this.prefix = prefix;
            connection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        }

        public void Publish(string eventName, ReadOnlyMemory<byte> eventData)
        {
            // TODO: cache declarations
            var exchangeName = GetFullExchangeName(eventName);
            using var channel = connection.Value.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            log.LogInformation($"decalre exchange '{exchangeName}' fanout");

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            var exchange = GetFullExchangeName(eventName);
            channel.BasicPublish(exchange: exchange, routingKey: string.Empty, body: eventData, basicProperties: properties);
            log.LogInformation($"publish to exchnage '{exchange}'");
        }

        public IDisposable Subscribe(string eventName, IEventBrokerHandler handler)
        {
            var queueName = GetFullQueueName(eventName, Assembly.GetEntryAssembly()!.GetName().Name!.ToLowerInvariant().Replace(".", "-"));
            var exchangeName = GetFullExchangeName(eventName);

            // TODO: cache declarations
            var channel = connection.Value.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: true);
            channel.QueueBind(queueName, exchangeName, routingKey: string.Empty);
            log.LogInformation($"declare queue '{queueName}' bound to exchange '{exchangeName}'");

            var rabbitConsumer = new RabbitConsumer(channel, handler, log);
            channel.BasicConsume(queueName, autoAck: false, rabbitConsumer);
            log.LogInformation($"consume queue '{queueName}'");
            return rabbitConsumer;
        }

        private string GetFullExchangeName(string eventName)
        {
            return $"{prefix}.{eventName}";
        }

        private string GetFullQueueName(string eventName, string handlerName)
        {
            return $"{prefix}.{eventName}.{handlerName}";
        }

        private class RabbitConsumer : DefaultBasicConsumer, IDisposable
        {
            private readonly ILogger<RabbitEventBroker> log;
            private readonly IEventBrokerHandler handler;
            private readonly IModel channel;

            public RabbitConsumer(IModel channel, IEventBrokerHandler handler, ILogger<RabbitEventBroker> log)
            {
                this.channel = channel;
                this.handler = handler;
                this.log = log;
            }

            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
            {
                log.LogInformation($"HandleBasicDeliver: routingKey {routingKey}");

                try
                {
                    handler.Handle(body);
                    channel.BasicAck(deliveryTag, false);
                }
                catch (Exception e)
                {
                    log.LogError(e, "handle failed");
                    channel.BasicNack(deliveryTag, false, false);
                    throw;
                }
            }

            public override void HandleModelShutdown(object model, ShutdownEventArgs reason)
            {
                log.LogInformation($"HandleModelShutdown: {reason.ReplyText}");
                base.HandleModelShutdown(model, reason);
            }

            public void Dispose()
            {
                channel.Dispose();
            }
        }
    }
}
