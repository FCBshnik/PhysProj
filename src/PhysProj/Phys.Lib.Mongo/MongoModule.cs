using Autofac;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Reader;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Lib.Mongo.Authors;
using Phys.Lib.Mongo.Files;
using Phys.Lib.Mongo.Users;
using Phys.Lib.Mongo.Works;
using Phys.Shared;
using Phys.Shared.Mongo;

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

            builder.Register(c => (UsersDb)c.ResolveNamed<IUsersDb>("mongo"))
                .As<IDbReader<UserDbo>>()
                .SingleInstance();
        }

        private void RegisterCollection<TModel, ImplDb, IDb>(ContainerBuilder builder, string collectionName) where ImplDb: Collection<TModel>, IDb
            where IDb : class
        {
            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<TModel>(collectionName))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ImplDb>()
                .As<IDb>().Named<IDb>(dbTypeName).AsImplementedInterfaces()
                .SingleInstance();

            //builder.Register(c => new NamedValue<TDb>(dbTypeName, c.ResolveNamed<TDb>(dbTypeName)))
            //    .As<INamedValue<TDb>>()
            //    .SingleInstance();
        }
    }
}
