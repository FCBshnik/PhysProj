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
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Files.Storage;
using Phys.Shared;

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
            var mongoUrl = configuration.GetConnectionString("mongo") ?? throw new PhysException($"connection string 'mongo' is missed");

            builder.RegisterModule(new MongoDbModule(mongoUrl, loggerFactory));
            builder.RegisterModule(new PostgresDbModule(configuration.GetConnectionString("postgres"), loggerFactory));

            // files
            var worksFilesPath = configuration.GetConnectionString("works-files");
            if (worksFilesPath != null)
                builder.Register(c => new LocalFileStorage("local", new DirectoryInfo(worksFilesPath), c.Resolve<ILogger<LocalFileStorage>>()))
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

            // history
            builder.Register(c => new MongoHistoryDbFactory(mongoUrl, "phys-lib", "history-", loggerFactory))
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
