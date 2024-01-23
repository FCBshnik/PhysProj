using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Lib.Mongo;
using Phys.Lib.Mongo.Authors;
using Phys.Lib.Mongo.Files;
using Phys.Lib.Mongo.Users;
using Phys.Lib.Mongo.Works;
using Phys.Mongo;

namespace Phys.Lib.Autofac
{
    public class MongoDbModule : Module
    {
        private const string dbTypeName = "mongo";

        private readonly MongoUrl url;
        private readonly ILogger log;

        public MongoDbModule(MongoUrl url, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(url);

            this.url = url;
            log = loggerFactory.CreateLogger<MongoDbModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.ConfigureConventions();

            log.LogInformation($"mongo connection: {url.Server}");
            var client = new MongoClient(url);

            builder.Register(_ => client.GetDatabase(url.DatabaseName ?? "phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterCollection<UserModel, UsersDb, IUsersDb>(builder, "users");
            RegisterCollection<AuthorModel, AuthorsDb, IAuthorsDb>(builder, "authors");
            RegisterCollection<WorkModel, WorksDb, IWorksDb>(builder, "works");
            RegisterCollection<FileModel, FilesDb, IFilesDb>(builder, "files");
        }

        private void RegisterCollection<TModel, ImplDb, IDb>(ContainerBuilder builder, string collectionName) where ImplDb : Collection<TModel>, IDb
            where IDb : class
            where TModel : MongoModel
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
