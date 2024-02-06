using Autofac;
using Microsoft.Extensions.Logging;
using Phys.Queue;
using Phys.RabbitMQ;
using RabbitMQ.Client;

namespace Phys.Lib.Autofac
{
    public class RabbitMqModule : Module
    {
        private readonly ILogger log;
        private readonly string connectionString;

        public RabbitMqModule(ILoggerFactory loggerFactory, string connectionString)
        {
            this.connectionString = connectionString;

            log = loggerFactory.CreateLogger<RabbitMqModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connectionFactory = new ConnectionFactory { Uri = new Uri(connectionString) };
            log.LogInformation($"rabbit server: {connectionFactory.HostName}:{connectionFactory.Port}");

            builder.Register(_ => connectionFactory)
                .As<IConnectionFactory>().SingleInstance();
            builder.RegisterType<RabbitQueue>()
                .WithParameter(TypedParameter.From("physlib"))
                .As<IMessageQueue>().SingleInstance();
            builder.RegisterType<JsonQueue>()
                .As<IQueue>().SingleInstance();
        }
    }
}
