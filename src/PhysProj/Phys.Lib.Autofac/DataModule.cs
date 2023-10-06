using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Files.Local;
using Phys.Files;
using Phys.Lib.Mongo;
using Phys.Lib.Postgres;
using Phys.Mongo.HistoryDb;
using Phys.Queue;
using Phys.RabbitMQ;
using RabbitMQ.Client;

namespace Phys.Lib.Autofac
{
    public class DataModule : Module
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public DataModule(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new MongoDbModule(configuration.GetConnectionString("mongo"), loggerFactory));
            builder.RegisterModule(new PostgresDbModule(configuration.GetConnectionString("postgres"), loggerFactory));

            builder.Register(c => new SystemFileStorage("local", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/files"), c.Resolve<ILogger<SystemFileStorage>>()))
                .As<IFileStorage>().SingleInstance();

            builder.Register(c => new MongoHistoryDbFactory(configuration.GetConnectionString("mongo"), "phys-lib", "history-", loggerFactory))
                .SingleInstance().AsImplementedInterfaces();

            builder.Register(c => new ConnectionFactory { HostName = configuration.GetConnectionString("rabbitmq") })
                .As<IConnectionFactory>().SingleInstance();
            builder.RegisterType<RabbitQueue>()
                .As<IMessageQueue>().SingleInstance();
            builder.RegisterType<JsonQueue>()
                .As<IObjectQueue>().SingleInstance();
        }
    }
}
