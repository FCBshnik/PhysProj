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
using Phys.Lib.Core;
using Phys.Mongo.Settings;
using MongoDB.Driver;
using Phys.Shared;
using System.Xml.Linq;

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
            var mongoNames = configuration.GetConnectionStringOrThrow("mongo").Split(",");
            if (mongoNames.Length < 1)
                throw new PhysException($"connection string 'mongo' must contain at least one mongo endpoint name");

            var mongoUrl = new MongoUrl(configuration.GetConnectionStringOrThrow(mongoNames[0]));
            var postgresUrl = configuration.GetConnectionString("postgres");
            var rabbitUrl = configuration.GetConnectionStringOrThrow("rabbitmq");
            var meilisearchUrl = configuration.GetConnectionStringOrThrow("meilisearch");
            var meilisearchMasterKey = configuration.GetConnectionStringOrThrow("meilisearch-master-key");

            foreach (var mongoName in mongoNames)
                builder.RegisterModule(new MongoDbModule(new MongoUrl(configuration.GetConnectionStringOrThrow(mongoName)), mongoName, loggerFactory));
            // postgres is optional
            if (postgresUrl != null)
                builder.RegisterModule(new PostgresDbModule(postgresUrl, loggerFactory));

            builder.RegisterModule(new MeilisearchModule(meilisearchUrl, meilisearchMasterKey, "phys-lib", loggerFactory));
            builder.RegisterModule(new RabbitMqModule(loggerFactory, rabbitUrl));

            builder.RegisterModule<SettingsModule>();

            // files
            // local file provider is optional
            var worksFilesPath = configuration.GetConnectionString("works-files");
            if (worksFilesPath != null)
                builder.Register(c => new LocalFileStorage("local", new DirectoryInfo(worksFilesPath), c.Resolve<ILogger<LocalFileStorage>>()))
                    .As<IFileStorage>().SingleInstance();
            builder.Register(c => RestService.For<IPCloudApiClient>("https://eapi.pcloud.com/"))
                .As<IPCloudApiClient>().SingleInstance();
            builder.RegisterType<PCloudFileStorage>()
                .As<IFileStorage>().SingleInstance();

            // history
            builder.Register(c => new MongoHistoryDbFactory(mongoUrl, mongoUrl.DatabaseName ?? "phys-lib", "history-", loggerFactory))
                .SingleInstance().AsImplementedInterfaces();

            // settings
            builder.Register(c => new MongoSettings(mongoUrl, mongoUrl.DatabaseName ?? "phys-lib", "settings", loggerFactory.CreateLogger<MongoSettings>()))
                .SingleInstance().AsImplementedInterfaces();
        }
    }
}
