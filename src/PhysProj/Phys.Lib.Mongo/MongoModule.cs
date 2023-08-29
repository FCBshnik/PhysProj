using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Phys.Lib.Mongo.Authors;
using Phys.Lib.Mongo.Files;
using Phys.Lib.Mongo.Users;
using Phys.Lib.Mongo.Works;

namespace Phys.Lib.Mongo
{
    public class MongoModule : Module
    {
        private readonly string connectionString;
        private readonly ILogger log;

        public MongoModule(string connectionString, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            this.connectionString = connectionString;
            log = loggerFactory.CreateLogger<MongoModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.Configure();

            log.LogInformation($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));

            builder.Register(_ => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterCollection<UserModel, UsersDb>(builder, "users");
            RegisterCollection<AuthorModel, AuthorsDb>(builder, "authors");
            RegisterCollection<WorkModel, WorksDb>(builder, "works");
            RegisterCollection<FileModel, FilesDb>(builder, "files");
        }

        private void RegisterCollection<TItem, TDb>(ContainerBuilder builder, string collectionName) where TDb: Collection<TItem>
        {
            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<TItem>(collectionName))
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<TDb>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
