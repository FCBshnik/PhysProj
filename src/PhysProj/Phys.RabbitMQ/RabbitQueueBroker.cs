using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Phys.Shared.Queue.Broker;

namespace Phys.RabbitMQ
{
    public class RabbitQueueBroker : IQueueBroker
    {
        private readonly ILogger<RabbitQueueBroker> log;
        private readonly Lazy<IConnection> connection;
        private readonly string queuePrefix;

        public RabbitQueueBroker(IConnectionFactory connectionFactory, string queuePrefix, ILogger<RabbitQueueBroker> log)
        {
            this.log = log;
            this.queuePrefix = queuePrefix;
            connection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        }

        public IDisposable Consume(string queueName, IQueueBrokerConsumer consumer)
        {
            queueName = GetFullQueueName(queueName);
            var channel = EnsureChannel(queueName);
            var rabbitConsumer = new RabbitConsumer(channel, consumer, log);
            channel.BasicConsume(queueName, autoAck: false, rabbitConsumer);
            log.LogInformation($"BasicConsume '{queueName}'");
            return rabbitConsumer;
        }

        public void Send(string queueName, ReadOnlyMemory<byte> message)
        {
            queueName = GetFullQueueName(queueName);
            using var channel = EnsureChannel(queueName);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: message, basicProperties: properties);
            log.LogInformation($"BasicPublish '{queueName}'");
        }

        private IModel EnsureChannel(string queueName)
        {
            var channel = connection.Value.CreateModel();
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            log.LogInformation($"QueueDeclare '{queueName}'");
            return channel;
        }

        private string GetFullQueueName(string queueName)
        {
            return $"{queuePrefix}.{queueName}";
        }

        private class RabbitConsumer : DefaultBasicConsumer, IDisposable
        {
            private readonly ILogger<RabbitQueueBroker> log;
            private readonly IQueueBrokerConsumer consumer;
            private readonly IModel channel;

            public RabbitConsumer(IModel channel, IQueueBrokerConsumer consumer, ILogger<RabbitQueueBroker> log)
            {
                this.channel = channel;
                this.consumer = consumer;
                this.log = log;
            }

            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
            {
                log.LogInformation($"HandleBasicDeliver: routingKey {routingKey}");

                try
                {
                    consumer.Consume(body);
                    channel.BasicAck(deliveryTag, false);
                }
                catch (Exception e)
                {
                    log.LogError(e, "consume failed");
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