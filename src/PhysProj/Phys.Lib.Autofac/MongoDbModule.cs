using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Phys.Lib.Db;
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
        private readonly MongoUrl url;
        private readonly string name;
        private readonly ILogger log;

        public MongoDbModule(MongoUrl url, string name, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(url);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            this.url = url;
            this.name = name;
            log = loggerFactory.CreateLogger<MongoDbModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.ConfigureConventions();

            log.LogInformation($"mongo '{name}' server: {url.Server}");
            var client = new MongoClient(url);

            builder.Register(_ => client.GetDatabase(url.DatabaseName ?? "phys-lib"))
                .Named<IMongoDatabase>(name)
                .SingleInstance();

            RegisterCollection<UserModel, UserDbo, UsersDb, IUsersDb>(builder, "users");
            RegisterCollection<AuthorModel, AuthorDbo, AuthorsDb, IAuthorsDb>(builder, "authors");
            RegisterCollection<WorkModel, WorkDbo, WorksDb, IWorksDb>(builder, "works");
            RegisterCollection<FileModel, FileDbo, FilesDb, IFilesDb>(builder, "files");
        }

        private void RegisterCollection<TModel, TDbo, ImplDb, IDb>(ContainerBuilder builder, string collectionName) where ImplDb : Collection<TModel>, IDb
            where IDb : class
            where TModel : MongoModel
        {
            builder.Register(c => c.ResolveNamed<IMongoDatabase>(name).GetCollection<TModel>(collectionName))
                .Named<IMongoCollection<TModel>>(name)
                .SingleInstance();

            builder.RegisterType<ImplDb>()
                .As<IDb>().Named<IDb>(name)
                .As<IDbReader<TDbo>>().Named<IDbReader<TDbo>>(name)
                .WithParameter(ResolvedParameter.ForNamed<Lazy<IMongoCollection<TModel>>>(name))
                .WithParameter(TypedParameter.From(name))
                .SingleInstance();
        }
    }
}
