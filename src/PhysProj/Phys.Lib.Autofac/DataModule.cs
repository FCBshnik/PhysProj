using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Files.Local;
using Phys.Files;
using Phys.Mongo.HistoryDb;
using Phys.Queue;
using Phys.RabbitMQ;
using RabbitMQ.Client;
using Phys.Files.PCloud;
using Refit;
using Phys.HistoryDb;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Files.Storage;

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

            // files
            builder.Register(c => new SystemFileStorage("local", "C:\\@yan\\dev\\projects\\physics\\dev\\files\\works\\", c.Resolve<ILogger<SystemFileStorage>>()))
                .As<IFileStorage>().SingleInstance();
            builder.Register(c => RestService.For<IPCloudApiClient>("https://eapi.pcloud.com/"))
                .As<IPCloudApiClient>().SingleInstance();
            builder.RegisterType<PCloudFileStorage>()
                .WithParameter(TypedParameter.From(new PCloudStorageSettings
                {
                    Username = "fcbshnik@gmail.com",
                    Password = "e331380e840b72810ec0fe230553da22",
                    BaseFolderId = 7289236945,
                }))
                .As<IFileStorage>().SingleInstance();
            builder.RegisterType<FilesContentMigrator>()
                .As<IMigrator>()
                .SingleInstance();

            // historyt
            builder.Register(c => new MongoHistoryDbFactory(configuration.GetConnectionString("mongo"), "phys-lib", "history-", loggerFactory))
                .SingleInstance().AsImplementedInterfaces();

            // rabbit
            builder.Register(c => new ConnectionFactory { HostName = configuration.GetConnectionString("rabbitmq") })
                .As<IConnectionFactory>().SingleInstance();
            builder.RegisterType<RabbitQueue>()
                .As<IMessageQueue>().SingleInstance();
            builder.RegisterType<JsonQueue>()
                .As<IObjectQueue>().SingleInstance();
        }
    }
}
