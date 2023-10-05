using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core;
using Phys.Shared.Logging;
using Phys.Shared.Queue;
using RabbitMQ.Client;
using Phys.Lib.Postgres;
using Phys.Lib.Mongo;
using Phys.Shared.RabbitMQ;
using Phys.Shared.Mongo.HistoryDb;
using Phys.Lib.Core.Migration;

namespace Phys.Lib.App
{
    internal class AppModule : Module
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfiguration configuration;

        public AppModule(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new LoggerModule(loggerFactory));

            builder.RegisterModule(new MongoModule(configuration.GetConnectionString("mongo"), loggerFactory));
            builder.RegisterModule(new PostgresModule(configuration.GetConnectionString("postgres"), loggerFactory));
            builder.Register(c => new MongoHistoryDbFactory(configuration.GetConnectionString("mongo"), "phys-lib", "history-", loggerFactory))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterModule(new CoreModule());

            builder.RegisterType<MigrationsExecutor>()
                .As<IHostedService>().SingleInstance();

            builder.Register(c => new ConnectionFactory { HostName = configuration.GetConnectionString("rabbitmq") })
                .As<IConnectionFactory>().SingleInstance();
            builder.RegisterType<RabbitQueue>()
                .As<IMessageQueue>().SingleInstance();
            builder.RegisterType<JsonQueue>()
                .As<IObjectQueue>().SingleInstance();
        }
    }
}
