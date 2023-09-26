using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Lib.Mongo.Authors;
using Phys.Lib.Mongo.Files;
using Phys.Lib.Mongo.Users;
using Phys.Lib.Mongo.Works;
using Phys.Shared.Mongo;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Mongo
{
    public class MongoModule : Module
    {
        private const string dbTypeName = "mongo";

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
            MongoConfig.ConfigureConventions();

            log.LogInformation($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));

            builder.Register(_ => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterCollection<UserModel, UsersDb, IUsersDb>(builder, "users");
            RegisterCollection<AuthorModel, AuthorsDb, IAuthorsDb>(builder, "authors");
            RegisterCollection<WorkModel, WorksDb, IWorksDb>(builder, "works");
            RegisterCollection<FileModel, FilesDb, IFilesDb>(builder, "files");
        }

        private void RegisterCollection<TModel, ImplDb, IDb>(ContainerBuilder builder, string collectionName) where ImplDb: Collection<TModel>, IDb
            where IDb : class
            where TModel: MongoModel
        {
            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<TModel>(collectionName))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ImplDb>()
                .As<IDb>().Named<IDb>(dbTypeName).AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
