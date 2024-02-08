using Microsoft.Extensions.Logging;
using Phys.Shared.EventBus.Broker;
using RabbitMQ.Client;

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
            using var channel = EnsureExchange(eventName);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: GetFullExchangeName(eventName), routingKey: string.Empty, body: eventData, basicProperties: properties);
            log.LogInformation($"BasicPublish '{eventName}'");
        }

        public IDisposable Subscribe(string eventName, IEventBrokerHandler handler)
        {
            var channel = EnsureQueue(eventName, handler.Name);
            var rabbitConsumer = new RabbitConsumer(channel, handler, log);
            channel.BasicConsume(GetFullQueueName(eventName, handler.Name), autoAck: false, rabbitConsumer);
            log.LogInformation($"BasicConsume '{eventName}'");
            return rabbitConsumer;
        }

        private IModel EnsureExchange(string eventName)
        {
            var exchangeName = GetFullExchangeName(eventName);
            var channel = connection.Value.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            log.LogInformation($"ExchangeDeclare '{exchangeName}'");
            return channel;
        }

        private IModel EnsureQueue(string eventName, string handlerName)
        {
            var queueName = GetFullQueueName(eventName, handlerName);
            var exchangeName = GetFullExchangeName(eventName);

            var channel = connection.Value.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: true);
            channel.QueueBind(queueName, exchangeName, routingKey: string.Empty);
            log.LogInformation($"QueueDeclare '{queueName}'");
            return channel;
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
