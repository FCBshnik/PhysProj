using Microsoft.Extensions.Logging;
using Phys.Shared.Queue;
using RabbitMQ.Client;
using System.Text;

namespace Phys.Shared.RabbitMQ
{
    public class RabbitQueue : IMessageQueue
    {
        private readonly ILogger<RabbitQueue> log;
        private readonly Lazy<IConnection> connection;

        public RabbitQueue(IConnectionFactory connectionFactory, ILogger<RabbitQueue> log)
        {
            this.log = log;
            connection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        }

        public IDisposable Consume(string queueName, IMessageConsumer consumer)
        {
            var channel = EnsureChannel(queueName);
            var rabbitConsumer = new RabbitConsumer(channel, consumer, log);
            channel.BasicConsume(queueName, autoAck: true, rabbitConsumer);
            log.LogInformation($"BasicConsume '{queueName}'");
            return rabbitConsumer;
        }

        public void Publish(string queueName, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            using var channel = EnsureChannel(queueName);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: body, basicProperties: properties);
            log.LogInformation($"BasicPublish '{queueName}'");
        }

        private IModel EnsureChannel(string queueName)
        {
            var channel = connection.Value.CreateModel();
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            log.LogInformation($"QueueDeclare '{queueName}'");
            return channel;
        }

        private class RabbitConsumer : DefaultBasicConsumer, IDisposable
        {
            private readonly ILogger<RabbitQueue> log;
            private readonly IMessageConsumer consumer;
            private readonly IModel channel;

            public RabbitConsumer(IModel channel, IMessageConsumer consumer, ILogger<RabbitQueue> log)
            {
                this.channel = channel;
                this.consumer = consumer;
                this.log = log;
            }

            public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
            {
                log.LogInformation($"HandleBasicDeliver: routingKey {routingKey}");
                var msg = Encoding.UTF8.GetString(body.Span);

                try
                {
                    consumer.Consume(msg);
                }
                catch (Exception e)
                {
                    log.LogError(e.Message, "consume failed");
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