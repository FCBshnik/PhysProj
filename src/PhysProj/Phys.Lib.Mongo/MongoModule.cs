using Autofac;
using MongoDB.Driver;
using NLog;
using Phys.Lib.Mongo.Authors;
using Phys.Lib.Mongo.Files;
using Phys.Lib.Mongo.Users;
using Phys.Lib.Mongo.Works;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Mongo
{
    public class MongoModule : Module
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string connectionString;

        public MongoModule(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.Configure();

            log.Info($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));

            builder.Register(_ => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterCollection<UserModel, UsersDb>(builder, "users");
            RegisterCollection<AuthorDbo, AuthorsDb>(builder, "authors");
            RegisterCollection<WorkDbo, WorksDb>(builder, "works");
            RegisterCollection<FileDbo, FilesDb>(builder, "files");
        }

        private void RegisterCollection<TItem, TDb>(ContainerBuilder builder, string collectionName) where TDb: Collection<TItem>
        {
            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<TItem>(collectionName))
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<TDb>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
